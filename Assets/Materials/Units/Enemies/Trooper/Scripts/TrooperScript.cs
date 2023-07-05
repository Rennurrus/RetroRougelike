using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrooperScript : MonoBehaviour
{
    [Header("Health Settings")]
    public int health;
    [SerializeField] private bool isDead = false;

    [Header("Movement Settings")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public float groundCheckDistance;

    [Header("Movement AI Settings")]
    public float runDistance;
    public float walkDistance;
    public float offsetDistance;
    public float stoppingDistance;
    public float retreatDistance;
    public float retreatStopingDistance;

    public float tempDistance;

    [SerializeField] private bool isGrounded;

    [Header("Shooting Settings")]
    public float timeBtwShots;
    public float startTimeBtwShots;
    public float timeCooldownBtwShots;
    public float startTimeCooldownBtwShots;
    public int   shotBurstSize;
    public int   burstCount;
    public GameObject bullet;
    public Transform shotPoint;

    private Animator animator;
    private Rigidbody2D rigidbody2d;

    private Transform player;
    private Transform arms;
    private Transform checkGroundPoint1;
    private Transform checkGroundPoint2;

    private Vector2 difference;
    private bool isFacingRight = true;
    private int rotMult = 1;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d     = GetComponent<Rigidbody2D>();
        animator        = GetComponent<Animator>();
        player          = GameObject.FindGameObjectWithTag("Player").transform;
        arms            = transform.GetChild(2).transform;
        checkGroundPoint1 = transform.GetChild(3).transform;
        checkGroundPoint2 = transform.GetChild(4).transform;

        bullet            = (GameObject) Resources.Load ("Bullets/EnemyBullet", typeof(GameObject));
        shotPoint         = transform.GetChild(2).GetChild(2).transform;

        burstCount        = shotBurstSize;
        //timeBtwShots      = startTimeBtwShots;
        //timeCooldownBtwShots = startTimeCooldownBtwShots;

        startTimeCooldownBtwShots = startTimeCooldownBtwShots + Random.Range(-2f,2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
             player          = GameObject.FindGameObjectWithTag("Player").transform;
        }
        animator.SetFloat("xVelocity", rigidbody2d.velocity.x * rotMult);
        animator.SetFloat("yVelocity", rigidbody2d.velocity.y);
    }

    private void FixedUpdate() 
    {
        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        float distanceY = Mathf.Abs(player.position.y - transform.position.y);

        if (CheckDistance(distanceX,distanceY))
        {
            Move(distanceX,distanceY);
            CheckRight();
            Aim();
        }
    }

    private bool CheckDistance(float distanceX, float distanceY)
    {
        return (distanceX <= tempDistance && distanceY <= tempDistance);
    }

    private void Move(float distanceX, float distanceY)
    {
        RaycastHit2D groundInfo1 = Physics2D.Raycast(checkGroundPoint1.position, Vector2.down, groundCheckDistance);
        RaycastHit2D groundInfo2 = Physics2D.Raycast(checkGroundPoint2.position, Vector2.down, groundCheckDistance);

        //float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        //float distanceY = Mathf.Abs(player.position.y - transform.position.y);
        if (!isDead)
        {
            if ( distanceX > runDistance && groundInfo1.collider)
            {
                Vector2 targetVelocity = new Vector2 ((player.position.x > transform.position.x  ? runSpeed : -runSpeed ) * Time.deltaTime , rigidbody2d.velocity.y);
                rigidbody2d.velocity = targetVelocity;
            }
            else if ( distanceX < walkDistance && distanceX > stoppingDistance && groundInfo1.collider)
            {
                Vector2 targetVelocity = new Vector2 ((player.position.x > transform.position.x  ? walkSpeed : -walkSpeed ) * Time.deltaTime, rigidbody2d.velocity.y);
                rigidbody2d.velocity = targetVelocity;
            }
            else if ( distanceX < retreatDistance && distanceY < retreatDistance && groundInfo2.collider)
            {
                Vector2 targetVelocity = new Vector2 ( (player.position.x > transform.position.x  ? -walkSpeed : walkSpeed ) * Time.deltaTime , rigidbody2d.velocity.y);
                rigidbody2d.velocity = targetVelocity;
            }
            else if ( (distanceX < stoppingDistance && distanceX > retreatStopingDistance) || distanceY > retreatStopingDistance || groundInfo2.collider == false || groundInfo1.collider == false)
            {
                Vector2 targetVelocity = new Vector2 (0 , rigidbody2d.velocity.y);
                rigidbody2d.velocity = targetVelocity;
            }
        }
        else
        {
            Vector2 targetVelocity = new Vector2 (0 , 0);
            rigidbody2d.velocity = targetVelocity;
        }


    }

    private void CheckRight()
    {
        if (!isDead)
            // Flip checking...
            if (!isFacingRight && player.position.x > transform.position.x)
            {
                Flip();
            }
            else if (isFacingRight && player.position.x < transform.position.x)
            {
                Flip();
            }
        
    }

    private void Aim() 
    {   
        difference = player.position - transform.position;
        float rotationZ = Mathf.Atan2(rotMult * difference.y , rotMult * difference.x) * Mathf.Rad2Deg;
        arms.rotation = Quaternion.Euler(0f , 0f, rotationZ);

        Shooting(rotationZ);
    }

    private void Shooting(float rotationZ)
    { 
        if (!isDead)
            if (timeCooldownBtwShots <= 0)
            {   
                if (burstCount > 0)
                {
                    if (timeBtwShots <= 0)
                    {
                        Instantiate(bullet, shotPoint.position, Quaternion.Euler(0f , 0f, rotationZ + (isFacingRight ? -90 : +90)));
                        timeBtwShots = startTimeBtwShots;
                        burstCount--;
                    }
                    else
                    {
                        timeBtwShots -= Time.deltaTime;
                    }
                }
                else
                {
                    timeCooldownBtwShots = startTimeCooldownBtwShots;
                    burstCount = shotBurstSize;
                }
            }
            else
            {
                timeCooldownBtwShots -= Time.deltaTime;
            }

    }

    private void Flip()
    {
        // Fliping all character...
        isFacingRight = !isFacingRight;
        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
        rotMult *= -1;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
         if (health <= 0)
        {
            animator.SetBool("isDead", true);
            isDead = true;
        }
    }
}