using UnityEngine;

public class Enemy2 : MonoBehaviour
{

    private enum Enemy2State {Alive , Dead};
    private Enemy2State currentEnemy2State;
    private enum Enemy2AliveState { Idle , Attack };
    private Enemy2AliveState currentEnemy2AliveState;
    

    [Header("Check For Player")]
    [SerializeField] private GameObject sense_Box;
    private Vector2 sense_Box_Size;
    [SerializeField] private float size_X;
    [SerializeField] private float size_Y;
    [SerializeField] private LayerMask player_Layer;

    [Header("Chase")]
    [SerializeField] private Transform player;
    [SerializeField] private float attackRange;
    [SerializeField] private float chaseSpeed;

    [Header("Attack")]
    [SerializeField] private Transform attackCircle;
    [SerializeField] private float attackRadius;

    [Header("TakeDamage")]
    [SerializeField] private int Health;
    [SerializeField] private GameObject Text;


    private bool lookingLeft;

    [Header("Compoments")]
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentEnemy2State = Enemy2State.Alive;
        currentEnemy2AliveState = Enemy2AliveState.Idle;
        animator = gameObject.GetComponent<Animator>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( currentEnemy2State == Enemy2State.Alive)
        {
            CheckForPlayer();

            if (currentEnemy2AliveState == Enemy2AliveState.Idle)
            {
                animator.SetBool("Attack" , false);
                animator.SetBool("Chase" , false);
            }
            else if (currentEnemy2AliveState == Enemy2AliveState.Attack)
            {
                Chase();
                Looking();

            }
        }
        else if (currentEnemy2State == Enemy2State.Dead)
        {
            
        }
    }

    private void CheckForPlayer()
    {
        sense_Box_Size = new Vector2( size_X ,size_Y );
        Collider2D sense_Area = Physics2D.OverlapBox(sense_Box.transform.position , sense_Box_Size , 0 , player_Layer );

        if (sense_Area)
        {
            currentEnemy2AliveState = Enemy2AliveState.Attack;
        }
        else
        {
            currentEnemy2AliveState = Enemy2AliveState.Idle;
        }
    }

    private void Chase()
    {
        //Debug.Log("Chase");
        Vector2 targetPos = new Vector2(player.position.x , transform.position.y);

        if (Vector2.Distance(transform.position , player.position) <= attackRange)
        {
            Attacking();
        }
        else
        {
            //Debug.Log("MoveTowards");
            transform. position = Vector2.MoveTowards(transform.position , targetPos , chaseSpeed * Time.deltaTime);
            animator.SetBool("Chase", true);
        }
    }
    private void Attacking()
    {
        animator.SetBool("Attack" , true);
        animator.SetBool("Chase" , false);

    }

    private void Attack()
    {
        Collider2D Attack_Circle = Physics2D.OverlapCircle(attackCircle.position , attackRadius , player_Layer);

        if (Attack_Circle)
        {
            PlayerController.instance.TakeDamage(1);
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
            Debug.Log("Enemy2 Health Is  = " + Health);

            Instantiate(Text , transform.position , Quaternion.identity);
        }
        CineMachineCamaraScript.instance.CameraShake(4,0.1f);
    }
    private void Die()
    {
        Debug.Log("Enemy2 Is Dead!");
        rigidbody2D.gravityScale = 0;
        boxCollider2D.enabled = false;
        animator.SetBool("Attack", false);
        animator.SetTrigger("Death");
        Instantiate(Text , gameObject.transform.position , Quaternion.identity);
        currentEnemy2State = Enemy2State.Dead;
        Destroy(gameObject , 6);
        Instantiate(Text , transform.position , Quaternion.identity);
        
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

    void OnDrawGizmosSelected()
    {
        if (sense_Box != null)
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireCube(sense_Box.transform.position , sense_Box_Size);
        }

        if (attackCircle != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCircle.position , attackRadius);
        }
    }


}
