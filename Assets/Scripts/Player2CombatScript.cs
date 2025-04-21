using UnityEngine;

public class Player2CombatScript : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    private int comboStep = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 0.1f;

    public GameObject LeftFootHitbox;
    public GameObject RightFootHitbox;
    public GameObject RightPunchHitbox;
    public Transform opponent;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravity = -20f;

    private Vector3 velocity;
    private bool isJumping = false;
    public bool isBlocking = false;
    private bool isAttacking = false;
    private bool isRoundOver = false;
    private bool isCrouched = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        comboStep = 0;
        animator.SetInteger("ComboStep", comboStep);
    }

    void Update()
    {
        if (isRoundOver) return;

        HandleJumping();
        HandleMovement();
        HandleFacingDirection();
        HandleBlocking();
        HandleAttack();

        // Apply gravity
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (!isJumping)
        {
            velocity.y = -1f;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFacingDirection()
    {
        if (opponent != null)
        {
            float directionToOpponent = opponent.position.x - transform.position.x;
            bool facingRight = Mathf.Approximately(transform.rotation.eulerAngles.y, 90);

            if ((directionToOpponent > 0 && !facingRight) || (directionToOpponent < 0 && facingRight))
            {
                transform.rotation = Quaternion.Euler(0, facingRight ? -90 : 90, 0);
            }
        }
    }

    void HandleAttack()
    {
        if (isJumping) return;

        if (Input.GetKeyDown(KeyCode.Keypad2)) // Attack input
        {
            Attack();
        }

        if (comboStep > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboResetTime)
            {
                ResetCombo();
                isAttacking = false;
            }
        }
    }

    void Attack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Fight Idle") || stateInfo.IsName("Cross Punch") || stateInfo.IsName("Martelo 2"))
        {
            isAttacking = true;
            comboStep++;
            if (comboStep > 3) comboStep = 0;

            animator.SetInteger("ComboStep", comboStep);
            comboTimer = 0f;
        }
    }

    void HandleMovement()
    {
        if (isJumping || isAttacking || isBlocking) return;

        float moveDirection = 0;
        float opponentDirection = Mathf.Sign(opponent.position.x - transform.position.x);
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) // Move Left (Backwards)
        {
            moveDirection = -1f;
            move = new Vector3(moveDirection, 0f, 0f) * moveSpeed;

            if (opponentDirection > 0)
            {
                animator.SetBool("MoveForward", false);
                animator.SetBool("MoveBackward", true);
            }
            else
            {
                animator.SetBool("MoveForward", true);
                animator.SetBool("MoveBackward", false);
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) // Move Right (Forwards)
        {
            moveDirection = 1f;
            move = new Vector3(moveDirection, 0f, 0f) * moveSpeed;

            if (opponentDirection > 0)
            {
                animator.SetBool("MoveBackward", false);
                animator.SetBool("MoveForward", true);
            }
            else
            {
                animator.SetBool("MoveBackward", true);
                animator.SetBool("MoveForward", false);
            }
        }
        else
        {
            animator.SetBool("MoveBackward", false);
            animator.SetBool("MoveForward", false);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            isCrouched = true;
            animator.SetBool("Crouch", true);
        }
        else
        {
            isCrouched = false;
            animator.SetBool("Crouch", false);
        }

        controller.Move(move * Time.deltaTime);
    }

    void HandleJumping()
    {
        if (isJumping || isAttacking || isBlocking || isCrouched) return;

        float opponentDirection = Mathf.Sign(opponent.position.x - transform.position.x);
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            isJumping = true;
            animator.ResetTrigger("Land");

            if (Input.GetKey(KeyCode.RightArrow)) // Forward flip
            {
                if (opponentDirection > 0)
                {
                    animator.SetTrigger("ForwardFlip");
                    velocity = new Vector3(moveSpeed, jumpForce, 0);
                }
                else
                {
                    animator.SetTrigger("BackFlip");
                    velocity = new Vector3(moveSpeed, jumpForce, 0);
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow)) // Backflip
            {
                if (opponentDirection > 0)
                {
                    animator.SetTrigger("BackFlip");
                    velocity = new Vector3(-moveSpeed, jumpForce, 0);
                }
                else
                {
                    animator.SetTrigger("ForwardFlip");
                    velocity = new Vector3(-moveSpeed, jumpForce, 0);
                }
            }
            else
            {
                animator.SetTrigger("Jump");
                velocity.y = jumpForce;
            }
        }

        if (controller.isGrounded && isJumping)
        {
            isJumping = false;
            velocity.x = 0f;
            animator.SetTrigger("Land");

        }
    }

    void HandleBlocking()
    {
        if (isJumping || isAttacking) return;

        if (Input.GetKey(KeyCode.Keypad1))
        {
            isBlocking = true;
        }
        else
        {
            isBlocking = false;
        }
    }

    void ResetCombo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", comboStep);
        comboTimer = 0f;
    }

    public void TriggerBlock()
    {
        animator.SetTrigger("Block");
    }
    public void TriggerDefeat()
    {

        animator.SetTrigger("Defeat");
    }

    public void TriggerVictory()
    {

        animator.SetTrigger("Victory");
    }

    void EnableRightPunchHitbox() => RightPunchHitbox.SetActive(true);
    void DisableRightPunchHitbox() => RightPunchHitbox.SetActive(false);
    void EnableLeftFootHitbox() => LeftFootHitbox.SetActive(true);
    void DisableLeftFootHitbox() => LeftFootHitbox.SetActive(false);
    void EnableRightFootHitbox() => RightFootHitbox.SetActive(true);
    void DisableRightFootHitbox() => RightFootHitbox.SetActive(false);
    public void TriggerHitStun() => animator.SetTrigger("HitStun");
}
