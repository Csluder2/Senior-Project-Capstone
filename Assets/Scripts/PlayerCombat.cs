using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private int comboStep = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 1f;

    public float attackDamage = 10f; // Damage per hit
    public Transform opponent; // Assign opponent in Unity

    private bool isBlocking = false;
    private bool isJumping = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        comboStep = 0;
        animator.SetInteger("ComboStep", comboStep);
    }

    void Update()
    {
        HandleMovement();
        HandleBlocking();
        HandleJumping();

        if (Input.GetKeyDown(KeyCode.P))
        {
            Attack();
        }

        if (comboStep > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboResetTime)
            {
                ResetCombo();
            }
        }
    }

    void HandleMovement()
    {
        if (isBlocking || isJumping) return; // Prevent movement while blocking or jumping

        if (Input.GetKey(KeyCode.D))
        {
            animator.Play("Walking");
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.Play("Slow Jog Backwards");
        }
        else
        {
            animator.Play("Fight Idle");
        }
    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.D))
            {
                animator.Play("Running Forward Flip");

            }
            else if (Input.GetKey(KeyCode.A))
            {
                animator.Play("Backflip");
            }
            else
            {
                animator.Play("Jumping Up");
            }
            isJumping = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isJumping)
        {
            animator.Play("Falling to Landing");
            isJumping = false;
        }
    }

    void HandleBlocking()
    {
        if (Input.GetKey(KeyCode.B))
        {
            animator.Play("Center Block");
            isBlocking = true;
        }
        else if (isBlocking)
        {
            isBlocking = false;
            animator.Play("Fight Idle");
        }
    }

    void Attack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Fight Idle") || stateInfo.IsName("Cross Punch") || stateInfo.IsName("Martelo 2"))
        {
            comboStep++;
            if (comboStep > 3) comboStep = 0;

            animator.SetInteger("ComboStep", comboStep);
            animator.SetTrigger("Attack");
            comboTimer = 0f;
        }
    }

    void ResetCombo()
    {
        comboStep = 0;
        animator.SetInteger("ComboStep", comboStep);
        comboTimer = 0f;
    }
}






