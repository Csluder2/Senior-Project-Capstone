using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    private int comboStep = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 0.1f;

    public GameObject LeftFootHitbox;
    public GameObject RightFootHitbox;
    public GameObject RightPunchHitbox;
    public Transform opponent;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isJumping = false;
    private bool isBlocking = false;
    private bool isAttacking = false;
    private bool isRoundOver = false;
    private bool isCrouched = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        comboStep = 0;
        animator.SetInteger("ComboStep", comboStep);
    }

    void Update()
    {


        HandleJumping();
        HandleMovement();
        HandleFacingDirection();
        HandleBlocking();
        HandleAttack();

    }

    void FixedUpdate()
    {

    }

    void HandleFacingDirection()
    {
        if (opponent != null)
        {
            float directionToOpponent = opponent.position.x - transform.position.x;
            bool facingRight = Mathf.Approximately(transform.rotation.eulerAngles.y, 90);

            if ((directionToOpponent > 0 && !facingRight) || (directionToOpponent < 0 && facingRight))
            {
                // Flip by rotating 180 degrees around the Y-axis
                transform.rotation = Quaternion.Euler(0, facingRight ? -90 : 90, 0);
            }
        }
    }

    void HandleAttack()
    {
        if (isJumping || isBlocking) return;

        if (Input.GetKeyDown(KeyCode.P)) // Attack input
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

    void EnableRightPunchHitbox()
    {

        RightPunchHitbox.SetActive(true);


    }

    void DisableRightPunchHitbox()
    {
        RightPunchHitbox.SetActive(false);


    }

    void EnableLeftFootHitbox()
    {
        LeftFootHitbox.SetActive(true);
    }

    void DisableLeftFootHitbox()
    {
        LeftFootHitbox.SetActive(false);
    }

    void EnableRightFootHitbox()
    {
        RightFootHitbox.SetActive(true);
    }

    void DisableRightFootHitbox()
    {
        RightFootHitbox.SetActive(false);
    }

    public void TriggerHitStun()
    {
        animator.SetTrigger("HitStun");
    }

    void HandleMovement()
    {
        if (isBlocking || isJumping || isAttacking) return;

        float moveDirection = 0;
        float opponentDirection = Mathf.Sign(opponent.position.x - transform.position.x);
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) // Move Left (Backwards)
        {
            moveDirection = -moveSpeed;
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
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) // Move Right (Forwards)
        {
            moveDirection = moveSpeed;
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

        if (Input.GetKey(KeyCode.S))
        {
            isCrouched = true;
            animator.SetBool("Crouch", true);
        }
        else
        {
            isCrouched = false;
            animator.SetBool("Crouch", false);
        }
        rb.linearVelocity = new Vector3(moveDirection, rb.linearVelocity.y, 0);
    }

    void HandleJumping()
    {
        if (isJumping || isAttacking || isBlocking || isCrouched) return;
        float opponentDirection = Mathf.Sign(opponent.position.x - transform.position.x);
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.D)) // Forward flip
            {
                isJumping = true;
                if (opponentDirection > 0)
                {
                    animator.SetTrigger("ForwardFlip");
                    rb.linearVelocity = new Vector3(moveSpeed, jumpForce, 0); // Forward + Jump
                }
                else
                {
                    animator.SetTrigger("BackFlip");
                    rb.linearVelocity = new Vector3(moveSpeed, jumpForce, 0);
                }
            }
            else if (Input.GetKey(KeyCode.A)) // Backflip
            {
                isJumping = true;
                if (opponentDirection > 0)
                {
                    animator.SetTrigger("BackFlip");
                    rb.linearVelocity = new Vector3(-moveSpeed, jumpForce, 0); // Backward + Jump
                }
                else
                {
                    animator.SetTrigger("ForwardFlip");
                    rb.linearVelocity = new Vector3(-moveSpeed, jumpForce, 0);
                }
            }

            else // Regular jump
            {
                isJumping = true;
                animator.SetTrigger("Jump");
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0); // Jump straight up
            }
        }
    }

    void HandleBlocking()
    {
        if (isJumping) return;

        if (Input.GetKeyDown(KeyCode.B) && !isBlocking)
        {
            isBlocking = true;
            animator.SetTrigger("Block");
            rb.linearVelocity = Vector3.zero; // Stop movement while blocking
        }
        else if (Input.GetKeyUp(KeyCode.B))
        {
            isBlocking = false;
            animator.SetTrigger("Idle");
        }
    }

    void ResetCombo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", comboStep);
        comboTimer = 0f;
    }

    public void TriggerDefeat()
    {
        isRoundOver = true;
        animator.SetTrigger("Defeat");
    }

    public void TriggerVictory()
    {
        isRoundOver = true;
        animator.SetTrigger("Victory");
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
        {
            isJumping = false;
            animator.SetTrigger("Land");
        }
    }
}








