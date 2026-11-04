using UnityEngine;
using UnityEngine.UI;

public class Enemy1 : MonoBehaviour
{

    [Header("Health")]
    [SerializeField] private int Health;

    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidbody2D;
    private int maxHealth;

    [Header("Attack")]
    [SerializeField] private float hurtCircleRadius;
    [SerializeField] private Transform hurtCircle;


    [Header("Collision Values")]
    [SerializeField] private Transform senseBox;
    [SerializeField]private float size_X;
    [SerializeField]private float attack_Size_X;
    private float normal_Size_X;
    [SerializeField]private float size_Y;
    private Vector2 size_SenseBox ;
    [SerializeField]private LayerMask playerLayer; 

    [Header("Chase Velues")]
    private Transform player;
    [SerializeField]private float retrieveDistance;
    [SerializeField]private float chaseSpeed;

    private Animator animator;
    private bool lookingLeft;
    [Header("Text")]
    [SerializeField]private GameObject Text;
    private Vector3 location;

    enum EnemyState
    {
        Alive,
        Dead
    }

    enum EnemyAliveState
    {
        Normal,
        Attack
    }
    EnemyState currentEnemyState;
    EnemyAliveState currentEnemyAliveState;



    void Start()
    {
        currentEnemyState = EnemyState.Alive;
        player = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        animator = gameObject.GetComponent<Animator>();
        normal_Size_X = size_X;
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
    CheckPlayer();
    location = transform.position;
        

        if (currentEnemyState == EnemyState.Alive)
        {
            if (currentEnemyAliveState == EnemyAliveState.Normal)
            {

                animator.SetBool("Chase" , false);
                animator.SetBool("Attack" , false);
                size_X = normal_Size_X;
                
            }
            else if (currentEnemyAliveState == EnemyAliveState.Attack)
            {
                size_X = attack_Size_X;

                Vector2 targetPos = new Vector2(player.position.x , transform.position.y);

                if (Vector2.Distance(transform.position, player.position) > retrieveDistance)
                {
                    transform.position = Vector2.MoveTowards(transform.position ,targetPos ,chaseSpeed * Time.deltaTime);
                    animator.SetBool("Chase" , true);
                    animator.SetBool("Attack" , false);
                }
                else
                {
                    //Debug.Log("#Take This# . !!How Is My Akes On Your Face!!");
                    animator.SetBool("Attack", true);
                }
                Looking();
            }
        }
        else if (currentEnemyState == EnemyState.Dead)
        {
            
        }
    }
    private void CheckPlayer()
    {
        size_SenseBox = new Vector2(size_X , size_Y);
        Collider2D collInfo = Physics2D.OverlapBox(senseBox.position , size_SenseBox , 0 , playerLayer);

        if (collInfo)
        {
            currentEnemyAliveState = EnemyAliveState.Attack;
            //Debug.Log("It Works!!!!!!");
        }
        else
        {
            currentEnemyAliveState = EnemyAliveState.Normal;
        }
    }

    private void Looking()
    {
        if (transform.position.x < player.position.x)
        {
            lookingLeft = false;
        }
        else
        {
            lookingLeft = true;
        }

        Flip();
    }

    private void Flip()
    {
        if (lookingLeft)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else
        {
            transform.eulerAngles = new Vector3 (0f, 180f ,0f);
        }
    }

    public void Attack()
    {
        Collider2D hitInfo = Physics2D.OverlapCircle(hurtCircle.position , hurtCircleRadius , playerLayer);

        if (hitInfo)
        {
            //Debug.Log("HitInfo if True");
            PlayerController Player = player.gameObject.GetComponent<PlayerController>();
            Player.TakeDamage(1);
            
        }
    }

    public void TakeDamage(int Damage)
    {
        if (Health <= 0)
        {
            Die();
        }
        else
        {
            Health -= Damage;
            animator.SetTrigger("Hurt");
            Debug.Log("Enemy1 Health Is  = " + Health);
            
            Vector3 location = transform.position;
            Instantiate(Text , location , Quaternion.identity);
        }
        CineMachineCamaraScript.instance.CameraShake(4,0.1f);
    }
    private void Die()
    {
        Debug.Log("Enemy1 Is Dead!");
        rigidbody2D.gravityScale = 0;
        boxCollider2D.enabled = false;
        animator.SetTrigger("Dead");
        Instantiate(Text , gameObject.transform.position , Quaternion.identity);
        currentEnemyState = EnemyState.Dead;
        Destroy(gameObject , 6);
        Instantiate(Text , location , Quaternion.identity);
        
    }

    void OnDrawGizmosSelected()
    {
        if (senseBox != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(senseBox.position , size_SenseBox);
        }


        if (hurtCircle != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hurtCircle.position , hurtCircleRadius);
        }
    }
}
