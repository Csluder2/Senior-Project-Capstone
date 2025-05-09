using UnityEngine;
using Fusion;
using Unity.Netcode.Components;
using Csluder2.FusionWork;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UIElements;



public class OnlinePlayerCombat : NetworkBehaviour, IBeforeUpdate
{
    private Animator animator;
    private NetworkMecanimAnimator netAnimator;
    private Rigidbody rb;
    private FusionConnection.PlayerInputData accumulatedInput;
    private bool resetInput;
    [SerializeField] private NetworkCharacterController ncc;
    [SerializeField] private float speed;
    [SerializeField] private float jump = 5f;


    [Networked] private int comboStep { get; set; }
    private float comboTimer = 0f;
    public float comboResetTime = 0.1f;

    public GameObject LeftFootHitbox;
    public GameObject RightFootHitbox;
    public GameObject RightPunchHitbox;
    public Transform opponent;

    public float moveSpeed = 4f;
    public float gravity = -20f;
    private Vector3 velocity = Vector3.zero;
    private bool isJumping = false;
    public bool isBlocking = false;
    private bool isAttacking = false;
    private bool isCrouched = false;
    private bool isRoundOver = false;
    private bool didMove;
    [Networked] private NetworkButtons PreviousButtons { get; set; }

    public override void Spawned()
    {
        animator = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkMecanimAnimator>();
        rb = GetComponent<Rigidbody>();

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

            yield return new WaitForSeconds(2f); // check again after delay
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        if (GetInput(out FusionConnection.PlayerInputData input))
        {


            HandleJumping(input);
            HandleMovement(input, ref didMove);
            HandleBlocking(input);
            HandleAttack(input);

        }
        HandleFacingDirection();

        if (!ncc.Grounded)
        {
            velocity.y += gravity * Runner.DeltaTime;
        }
        else if (!isJumping)
        {
            velocity.y = -1f;
        }


        ncc.Move(velocity * Runner.DeltaTime);


        if (!isJumping && !didMove)
        {
            ncc.Move(Vector3.zero);
        }
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
        if (isJumping) return;


        if (input.Buttons.WasPressed(PreviousButtons, (int)FusionConnection.PlayerButtons.Attack))
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

        PreviousButtons = input.Buttons;
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

    void HandleMovement(FusionConnection.PlayerInputData input, ref bool didMove)
    {
        if (!ncc.Grounded || isAttacking) return;

        float moveDirection = 0f;
        Vector3 HorizontalMovement = Vector3.zero;
        float opponentDirection = Mathf.Sign(opponent.position.x - transform.position.x);
        bool moveLeft = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.MoveLeft);
        bool moveRight = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.MoveRight);
        bool crouch = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.Crouch);
        if (moveLeft && !moveRight)
        {
            moveDirection = -1f;
            HorizontalMovement = new Vector3(moveDirection, 0, 0);
            didMove = true;

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
            moveDirection = 1f;
            HorizontalMovement = new Vector3(moveDirection, 0, 0);
            didMove = true;

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
            didMove = false;
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
        //Vector3 HorizontalMovement = new Vector3(moveDirection * speed, 0f, 0f);
        ncc.Move(HorizontalMovement * Runner.DeltaTime);

    }


    void HandleJumping(FusionConnection.PlayerInputData input)
    {
        if (isAttacking || isBlocking || isCrouched) return;
        bool jumping = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.Jump);
        bool moveLeft = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.MoveLeft);
        bool moveRight = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.MoveRight);
        float opponentDirection = Mathf.Sign(opponent.position.x - transform.position.x);

        if (jumping && ncc.Grounded)
        {
            if (moveRight)
            {

                if (opponentDirection > 0)
                {
                    netAnimator.SetTrigger("ForwardFlip");

                }
                else
                {
                    netAnimator.SetTrigger("BackFlip");

                }

            }
            else if (moveLeft)
            {

                if (opponentDirection > 0)
                {
                    netAnimator.SetTrigger("BackFlip");

                }
                else
                {
                    netAnimator.SetTrigger("ForwardFlip");

                }

            }
            else
            {
                netAnimator.SetTrigger("Jump");


            }
            ncc.Jump();
            isJumping = true;
            animator.ResetTrigger("Land");
        }


        if (ncc.Grounded && isJumping)
        {
            isJumping = false;
            netAnimator.SetTrigger("Land");
        }
    }

    void HandleBlocking(FusionConnection.PlayerInputData input)
    {
        if (isJumping || isAttacking) return;
        bool blocking = input.Buttons.IsSet((int)FusionConnection.PlayerButtons.Block);
        if (blocking)
            isBlocking = true;
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
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TriggerDefeat()
    {
        isRoundOver = true;
        netAnimator.SetTrigger("Defeat");

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TriggerBlock()
    {
        netAnimator.SetTrigger("Block");
        isBlocking = false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TriggerVictory()
    {
        isRoundOver = true;
        netAnimator.SetTrigger("Victory");
    }

    /*void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
        {
            isJumping = false;
            netAnimator.SetTrigger("Land");
        }
    }
    */
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TriggerHitStun()
    {
        netAnimator.SetTrigger("HitStun");
    }
    public void EnableRightPunchHitbox() => RightPunchHitbox.SetActive(true);
    public void DisableRightPunchHitbox() => RightPunchHitbox.SetActive(false);
    public void EnableLeftFootHitbox() => LeftFootHitbox.SetActive(true);
    public void DisableLeftFootHitbox() => LeftFootHitbox.SetActive(false);
    public void EnableRightFootHitbox() => RightFootHitbox.SetActive(true);
    public void DisableRightFootHitbox() => RightFootHitbox.SetActive(false);

    public void BeforeUpdate()
    {

        if (resetInput)
        {
            resetInput = false;
            accumulatedInput = default;
        }

        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            NetworkButtons buttons = default;
            buttons.Set(FusionConnection.PlayerButtons.Attack, true);
            accumulatedInput.Buttons = new NetworkButtons(accumulatedInput.Buttons.Bits | buttons.Bits);
        }

    }

}
