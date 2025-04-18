using UnityEngine;
using Fusion;
using Unity.Netcode.Components;
using Csluder2.FusionWork;
using UnityEngine.InputSystem;
using System.Collections;



public class OnlinePlayerCombat : NetworkBehaviour
{
    private Animator animator;
    private NetworkMecanimAnimator netAnimator;
    private Rigidbody rb;

    private NetworkRigidbody body;

    [Networked] private int comboStep { get; set; }
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
    private bool isCrouched = false;

    private bool isRoundOver = false;

    public override void Spawned()
    {
        animator = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkMecanimAnimator>();
        rb = GetComponent<Rigidbody>();
        body = GetComponent<NetworkRigidbody>();
        comboStep = 0;
        animator.SetInteger("ComboStep", comboStep);
        if (Object.HasInputAuthority)
        {
            enabled = true;
        }
        StartCoroutine(WaitForOpponent());

    }




    private IEnumerator WaitForOpponent()
    {
        while (true)
        {
            var players = FindObjectsOfType<OnlinePlayerCombat>();
            if (players.Length == 2)
            {
                foreach (var player in players)
                {
                    if (player != this)
                    {
                        opponent = player.transform;
                        yield break;
                    }
                }
            }

            yield return new WaitForSeconds(0.1f); // check again after delay
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;
        if (rb.isKinematic == true)
        {
            rb.isKinematic = false;
        }
        if (GetInput(out FusionConnection.PlayerInputData input))
        {
            HandleJumping(input);
            HandleMovement(input);
            HandleBlocking(input);
            HandleAttack(input);
        }
        HandleFacingDirection();

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

    void HandleAttack(FusionConnection.PlayerInputData input)
    {
        if (isJumping || isBlocking) return;

        bool attacking = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.Attack);
        if (attacking)
        {
            Attack();
        }

        if (comboStep > 0)
        {
            comboTimer += Runner.DeltaTime;
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

    void HandleMovement(FusionConnection.PlayerInputData input)
    {
        if (isBlocking || isJumping || isAttacking) return;


        float moveDirection = 0;
        float opponentDirection = Mathf.Sign(opponent.position.x - transform.position.x);
        bool moveLeft = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.MoveLeft);
        bool moveRight = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.MoveRight);
        bool crouch = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.Crouch);
        if (moveLeft && !moveRight)
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
        else if (moveRight && !moveLeft)
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

        if (crouch)
        {
            isCrouched = true;
            animator.SetBool("Crouch", true);
        }
        else
        {
            isCrouched = false;
            animator.SetBool("Crouch", false);
        }

        rb.linearVelocity = new Vector3(moveDirection, 0, 0);
    }


    void HandleJumping(FusionConnection.PlayerInputData input)
    {
        if (isJumping || isAttacking || isBlocking || isCrouched) return;
        bool jumping = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.Jump);
        bool moveLeft = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.MoveLeft);
        bool moveRight = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.MoveRight);
        float opponentDirection = Mathf.Sign(opponent.position.x - transform.position.x);

        if (jumping)
        {
            isJumping = true;
            if (moveRight)
            {
                if (opponentDirection > 0)
                {
                    netAnimator.SetTrigger("ForwardFlip");
                    rb.linearVelocity = new Vector3(moveSpeed, jumpForce, 0) * Runner.DeltaTime;
                }
                else
                {
                    netAnimator.SetTrigger("BackFlip");
                    rb.linearVelocity = new Vector3(moveSpeed, jumpForce, 0);
                }
            }
            else if (moveLeft)
            {
                if (opponentDirection > 0)
                {
                    netAnimator.SetTrigger("BackFlip");
                    rb.linearVelocity = new Vector3(-moveSpeed, jumpForce, 0);
                }
                else
                {
                    netAnimator.SetTrigger("ForwardFlip");
                    rb.linearVelocity = new Vector3(-moveSpeed, jumpForce, 0);
                }
            }
            else
            {
                netAnimator.SetTrigger("Jump");
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0);
            }
        }
    }

    void HandleBlocking(FusionConnection.PlayerInputData input)
    {
        if (isJumping) return;
        bool blocking = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.Block);
        if (blocking && !isBlocking)
        {
            isBlocking = true;
            netAnimator.SetTrigger("Block");
            rb.linearVelocity = Vector3.zero;
        }
        else if (!blocking)
        {
            isBlocking = false;
            netAnimator.SetTrigger("Idle");
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
        netAnimator.SetTrigger("Defeat");
    }

    public void TriggerVictory()
    {
        isRoundOver = true;
        netAnimator.SetTrigger("Victory");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
        {
            isJumping = false;
            netAnimator.SetTrigger("Land");
        }
    }

    public void EnableRightPunchHitbox() => RightPunchHitbox.SetActive(true);
    public void DisableRightPunchHitbox() => RightPunchHitbox.SetActive(false);
    public void EnableLeftFootHitbox() => LeftFootHitbox.SetActive(true);
    public void DisableLeftFootHitbox() => LeftFootHitbox.SetActive(false);
    public void EnableRightFootHitbox() => RightFootHitbox.SetActive(true);
    public void DisableRightFootHitbox() => RightFootHitbox.SetActive(false);
}
