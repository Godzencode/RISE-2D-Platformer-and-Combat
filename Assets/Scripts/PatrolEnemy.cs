using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{

    private enum EnemyState1 {Alive , Dead }
    private enum Alive_States {Patrol , Idle , Attack}



    [Header("Check Player")]
    [SerializeField]
    private Transform checkPoint;
    [SerializeField]
    private float size_X = 10;
    private float regular_size_X;
    private float attack_size_X = 20;
    [SerializeField]
    private float size_Y = 2f;
    [SerializeField]
    private LayerMask PlayerLayer;

    [Header("Ground Checker")]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private float patrolSpeed = 5f;
    [SerializeField]
    private float distance = 0.5f;
    [SerializeField]
    private Transform groundCheckPoint;
    [SerializeField]
    private LayerMask Ground;
    private LayerMask Platform;
    [SerializeField]
    private bool facingLeft;

    [Header("Attack ")]
    public GameObject Player;
    public float chaseSpeed;
    [SerializeField]private float retrieceDistance = 2.5f;

    private Animator animator;

    [SerializeField] private float attackStartTime =1f;
    private float TimeBetweenAttacks;

    [Header("Attack Details")]

    [SerializeField] private Transform hurtCircle;
    [SerializeField] private float hurtCircleRadius;
    [SerializeField] private float bigHurtAttackRadius = 1.5f;
    [SerializeField] private float smallHurtAttackRadius = 1.2f;

    [Header("Health")]
    [SerializeField] private int health;
    private int maxHealth;

    EnemyState1 currentState;
    Alive_States currentAliveState;
    private BoxCollider2D boxCollider2D;
    private Transform player;

    [Header("Text")]
    [SerializeField]private GameObject Text;
    private Vector3 location;

    


    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        TimeBetweenAttacks = attackStartTime;

        currentState = EnemyState1.Alive;
        currentAliveState = Alive_States.Patrol;
        regular_size_X = size_X;
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        
    }



    void Update()
    {
        location = transform.position;
        if (currentState == EnemyState1.Alive)
        {
            Collider2D collInfo = Physics2D.OverlapBox(checkPoint.position, new Vector2 (size_X,size_Y) , 0 , PlayerLayer);
            CheckGround();


            if (collInfo)
            {
                currentAliveState = Alive_States.Attack;
            }
            else if (!collInfo)
            {
                currentAliveState = Alive_States.Patrol;
            }
            

            if (currentAliveState == Alive_States.Patrol)
            {
                
                rigidbody2D.linearVelocity = -transform.right * patrolSpeed;
                size_X = regular_size_X;
                animator.SetBool("Idle" , false);

            }


            else if (currentAliveState == Alive_States.Attack)
            {
                animator.SetBool("Idle" ,false);
                size_X = attack_size_X;
                if (Vector2.Distance(transform.position, Player.transform.position) > retrieceDistance)
                {
                    Vector2 targetPos = new Vector2( Player.transform.position.x , transform.position.y );
                    transform.position = Vector2.MoveTowards( transform.position , targetPos , chaseSpeed * Time.deltaTime );


                    if (transform.position.x > Player.transform.position.x)
                    {
                        facingLeft = true;
                        TurnEnemy();
                    }
                    else if (transform.position.x < Player.transform.position.x)
                    {
                        facingLeft = false;
                        TurnEnemy();
                    }
                }
                
                else
                {
                animator.SetBool("Idle", true);

                if (TimeBetweenAttacks <= 0f )
                {
                    AttackAnimations();
                    TimeBetweenAttacks = attackStartTime;
                }
                else
                {
                    TimeBetweenAttacks -= Time.deltaTime;
                }

                //Debug.Log("Attack on Player Because Enemy is close to Player");
                }
            
            }


            //Debug.Log("collInfo if True");
        }
        else if (currentState == EnemyState1.Dead)
            {
                
            }   
    }


    void OnDrawGizmosSelected()
    {
        if(groundCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(groundCheckPoint.position , Vector2.down * distance);
        }

        if (checkPoint != null)
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireCube(checkPoint.position , new Vector2(size_X, size_Y));
        }

        if (hurtCircle != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hurtCircle.position , hurtCircleRadius);
        }
    }
    private void CheckGround()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(groundCheckPoint.position , Vector2.down , distance , Ground);

        


        if (!hitInfo)
        {
            TurnEnemy();
            
        }
    }
    private void TurnEnemy()
    {
        if (facingLeft)
            {
                transform.eulerAngles = Vector3.zero;
                facingLeft = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0 , 180 , 0);
                facingLeft = true;
            }
    }

    private void AttackAnimations()
    {
        int randomAttackNum = Random.Range(0,3);
        
        if (randomAttackNum == 0)
        {
            animator.SetTrigger("Attack1");
            
        }
        else if (randomAttackNum == 1 )
        {
            animator.SetTrigger("Attack2");
        }
        else
        {
            animator.SetTrigger("Attack3");
        }
    }
    private void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(hurtCircle.position, hurtCircleRadius , PlayerLayer);

        if (collInfo)
        {
            Debug.Log("CollInfo is true");
            PlayerController Player = player.gameObject.GetComponent<PlayerController>();
            if (Player != null)
            {
                Debug.Log("Playe is not null");
                Player.TakeDamage(1);
                
            }
            
        }
    }
    public void BigAttack()
    {
        hurtCircleRadius = bigHurtAttackRadius;
        Attack();
    }

    public void SmallAttack()
    {
        hurtCircleRadius = smallHurtAttackRadius;
        Attack();
    }

    public void TakeDamage(int Damage)
    {
        if (health <=0 )
        {
            Death();
            
        }
        else
        {
            animator.SetTrigger("Hurt");
            health -= Damage;
            Debug.Log("Current Health amount " + health);
            CineMachineCamaraScript.instance.CameraShake(4,0.25f);
            Instantiate(Text , location , Quaternion.identity);
        }
        
    }

    private void Death()
    {
        animator.SetTrigger("Death");
        currentState = EnemyState1.Dead;
        Debug.Log(gameObject.name + " is Dead");
        rigidbody2D.gravityScale = 0;
        boxCollider2D.enabled = false;
        Instantiate(Text , location , Quaternion.identity);

        Destroy(gameObject , 7f);
    }


}
