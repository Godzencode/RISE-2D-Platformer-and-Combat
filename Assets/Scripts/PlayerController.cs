using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;

    [Header("Heath")]
    [SerializeField] private Slider HealthBar;
    [SerializeField] private int Health;
    [SerializeField] private int MaxHealth;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject collect_Effect;

    
    [SerializeField]
    private Transform attackHitBoxPosition;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private float attackRadius;
    [SerializeField]
    private float attack1_1Radius;
    [SerializeField]
    private float attack1_2Radius;
    [SerializeField]private float attack2Radius;
    [SerializeField]private Transform attack2Position;
    private Vector2 attackPosition ;


    public float xMovement;
    public float speed = 100f;
    public float jumpHeight;
    public float newJumpHeight;
    private float originalJumpHeight;


    private  int turnAngle = 180;
    public int jumpCounter = 2;
    private int rightTurnangle = 0;
    private int leftTurnAngle = 180;


    private bool canJump;


    private Rigidbody2D rigidbody2D;
    private Animator animator;
    [Header("Damage")]
    [SerializeField] private int Damage;

    private bool attack1;

    enum PlayerState { Alive ,Death};
    PlayerState currentState ;

    private BoxCollider2D boxCollider2D;
    [SerializeField] private GameObject CineMachineCamera;
    private CineMachineCamaraScript CameraScript;
    [Header("DustEffect")]

    [SerializeField]private GameObject dustEffect_Prefab;
    [SerializeField]private GameObject dustEffect_SpawnPoint;

    [Header("PlayerDash")]

    [SerializeField]private float dashForce = 15f;
    [SerializeField]private float dashDuration = 0.12f;

    private float dashDirection;
    private bool isDashing;

    private bool isOnGround;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }



    void Start()
    {
        isOnGround = true;
        isDashing = false;
        HealthBar.maxValue = MaxHealth;
        HealthBar.value = Health;
        HealthBar.minValue = 0;

        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        originalJumpHeight = jumpHeight;
        currentState = PlayerState.Alive;
        boxCollider2D = GetComponentInChildren<BoxCollider2D>();
        CameraScript = CineMachineCamera.gameObject.GetComponent<CineMachineCamaraScript>();

    }

    // Update is called once per frame
    void Update()
    {

        if (currentState == PlayerState.Alive)
        {
            if (Collect.instance.hasKey)
            {
                animator.SetFloat( "Run" , 0);
                speed = 0;
                return;
            }
            PlayerMovement();

        PlayerAnimationRun();

        if (Input.GetMouseButtonDown(0))
        {
            PlayerAnimationAttack(1);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            PlayerAnimationAttack(2);
        }

        if (Input.GetKeyDown(KeyCode.V) && !isDashing )
        {
            StartCoroutine(Dash());
            Debug.Log("Dashing");
        }


        }
        else if (currentState == PlayerState.Death)
        {
            canJump = false;
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (currentState == PlayerState.Alive)
        {
            
        rigidbody2D.linearVelocity = new Vector2( xMovement * speed , rigidbody2D.linearVelocity.y);
        }
        else if (currentState == PlayerState.Death)
        {
            
        }
    }

    private void Jump()
    {
        AudioManager.instance.PlaySound("Jump");
        rigidbody2D.linearVelocityY = jumpHeight;
        PlayerAnimationJump();
        GameObject Dust = Instantiate(dustEffect_Prefab , dustEffect_SpawnPoint.transform.position , Quaternion.identity);
        Destroy(Dust , 0.2f);
        jumpCounter = jumpCounter - 1;
        if (jumpCounter == 1)
        {
            jumpHeight = newJumpHeight;
        }
        
        if (jumpCounter == 0)
        {
            canJump =false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform")
        {
            //Debug.Log("You hit the ground");
            isOnGround = true;  
            canJump = true;
            jumpCounter = 2;
            jumpHeight = originalJumpHeight;
            animator.SetBool("OnGround", true);
             
        }
        else
        {
            isOnGround = false;
            Debug.Log("isOnGround is False");
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Trap"))
        {
            TakeDamage(1);
        }     

        if (collision.gameObject.CompareTag("Heart"))
        {
            if (Health < MaxHealth)
            {
                AudioManager.instance.PlaySound("HeartCollect");
                Health++;
                HealthBar.value = Health;
                Destroy(collision.gameObject);

                GameObject temp = Instantiate(collect_Effect, collision.gameObject.transform.position, Quaternion.identity);
                Destroy(temp, 0.5f);

            }
        }  

        if (collision.gameObject.CompareTag("DeadLine"))
        {
            Death();
        } 
        
    }


    private void PlayerAnimationRun()
    {
        if (Mathf.Abs(xMovement) > 0 )
        {
            animator.SetFloat("Run" , 1);
        }
        else
        {
            animator.SetFloat("Run" , 0);
        }
    }

    private void PlayerAnimationJump()
    {
        animator.SetTrigger( "Jump 0");
        animator.SetBool("OnGround", false);
    }

    private void PlayerAnimationAttack(int Attack)
    {
        if (Attack == 1)
        {
            animator.SetTrigger("Attack1");
        }
        else if (Attack == 2)
        {
            animator.SetTrigger("Attack2");
        }
    }

    private void PlayerAnimationHurt()
    {
        animator.SetTrigger("Hurt");
    }

    private void Attack()
    {
        AudioManager.instance.PlaySound("Attack");
        Collider2D collisionInfo;
        if (attack1)
        {
            collisionInfo = Physics2D.OverlapCircle( attackHitBoxPosition.position , attackRadius , enemyLayer);
        }
        else
        {
            collisionInfo = Physics2D.OverlapCircle( attack2Position.position, attack2Radius , enemyLayer);

        }
        
        

        if (collisionInfo == true )
        {
            if (collisionInfo.gameObject.GetComponent<PatrolEnemy>() != null)
            {
                collisionInfo.gameObject.GetComponent<PatrolEnemy>().TakeDamage(Damage);
            }
            if (collisionInfo.gameObject.GetComponent<Enemy1>() != null)
            {
                collisionInfo.gameObject.GetComponent<Enemy1>().TakeDamage(Damage);
            }
            if (collisionInfo.gameObject.GetComponent<Enemy2>() != null)
            {
                collisionInfo.gameObject.GetComponent<Enemy2>().TakeDamage(1);
            }
        }
    }
    public void Attack1Fram1()
    {
        attackRadius = attack1_1Radius;
        attack1 = true;

        Attack();
    }
    public void Attack1Fram2()
    {
        attackRadius = attack1_2Radius;
        attack1 = true;
        Attack();
    }
    public void Attack2()
    {
        attack1 = false;
        Attack();
    }

    void OnDrawGizmosSelected()
    {
        if (attackHitBoxPosition != null )
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackHitBoxPosition.position, attackRadius);
        }
        if (attack2Position != null )
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attack2Position.position, attack2Radius);
        }
    }

    public void TakeDamage(int Damage)
    {
        if (Health < 1)
        {
            Death();
        }
        else
        {
            Health -= Damage;
            HealthBar.value = Health;
            animator.SetTrigger("Hurt");
            CineMachineCamaraScript.instance.CameraShake(4,0.1f);
        }
    }
    private void Death()
    {
        
        rigidbody2D.gravityScale = 0;
        boxCollider2D.enabled = false;
        animator.SetTrigger("Death");
        currentState = PlayerState.Death;
        GameManager.instance.TriggerLosePanel();
        Destroy(gameObject);
        Instantiate(explosionPrefab , dustEffect_SpawnPoint.transform.position , Quaternion.identity);
        
    }



    private void PlayerMovement()
    {

        xMovement = Input.GetAxis("Horizontal");
        if (xMovement < 0 )
        {
            turnAngle = leftTurnAngle;
            transform.rotation = new Quaternion(transform.rotation.x , turnAngle, transform.rotation.z, transform.rotation.w);
            dashDirection = -1f;
        }
        else if (xMovement > 0 )
        {
            turnAngle = rightTurnangle;
            transform.rotation = new Quaternion(transform.rotation.x , turnAngle, transform.rotation.z, transform.rotation.w);
            dashDirection = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            Jump();
        }
    }
    IEnumerator Dash()
        {
            isDashing = true;
            float savedGravity = rigidbody2D.gravityScale;
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.linearVelocity = new Vector2(dashDirection * dashForce, rigidbody2D.linearVelocity.y);
            yield return new WaitForSeconds(dashDuration);
            rigidbody2D.gravityScale = savedGravity;
            isDashing = false;
        }
}


