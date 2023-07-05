 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;

    
    public Joystick moveJoystick;
    public Joystick aimJoystick;
    public enum ControlType{ PC, Android };
    public bool isGrounded;
    public Aiming aiming;
    public FollowCamera followCamera;
    public Animator animator;

    [Range(0, 5f)] [SerializeField] private float fallLongMult = 0.85f;
    [Range(0, 5f)] [SerializeField] private float fallShortMult = 1.55f;

    private Rigidbody2D rigidbody2d;
    private float horizontalValue;
    private float verticalValue;
    [SerializeField] private bool isJumped = false;
    private bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()    
    {
        rigidbody2d     = GetComponent<Rigidbody2D>();
        animator        = GetComponent<Animator>();
        moveJoystick    = GameObject.Find("Canvas").GetComponent<JoysticksObjects>().moveJoystick;
        aimJoystick     = GameObject.Find("Canvas").GetComponent<JoysticksObjects>().aimJoystick;
        followCamera    = GameObject.Find("Main Camera").GetComponent<FollowCamera>();
        followCamera.setTarget(transform);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalValue = moveJoystick.Horizontal;
        verticalValue   = moveJoystick.Vertical;
        animator.SetFloat("yVelocity", rigidbody2d.velocity.y);
    }

    private void FixedUpdate() 
    {
        Move(horizontalValue);
        Jump(verticalValue);
        CheckRight(moveJoystick, aimJoystick);
    }

    private void Move(float horizontal)
    {
        // Running...
        if ( horizontal > 0.6f || horizontal < -0.6f)
        {
            Vector2 targetVelocity = new Vector2 ( (horizontal > 0 ? 1 : -1) * runSpeed * Time.deltaTime , rigidbody2d.velocity.y);
            rigidbody2d.velocity = targetVelocity;
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
        }
        // Walking...
        else if ( horizontal > 0.2f || horizontal < -0.2f)
        {
            Vector2 targetVelocity = new Vector2 ( (horizontal > 0 ? 1 : -1) * walkSpeed * Time.deltaTime , rigidbody2d.velocity.y);
            rigidbody2d.velocity = targetVelocity;
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", true);
        }
        // Stop...
        else 
        {
            Vector2 targetVelocity = new Vector2 (0 , rigidbody2d.velocity.y);
            rigidbody2d.velocity = targetVelocity;
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
        }
    }

    private void Jump(float vertical)
    {
        // Jump checking...
        if (vertical >= 0.5f && isGrounded && isJumped == false)
        {
            rigidbody2d.AddForce(new Vector2(0,  jumpForce ), ForceMode2D.Impulse);
            isJumped = true;
        }

        // Multijump locking...
        if (vertical < 0.5f && isJumped)
        {
            isJumped = false;
        }

        // Jumping High...
        if(vertical >= 0.5f && rigidbody2d.velocity.y > 0)
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity.y * (fallLongMult - 1) * Time.fixedDeltaTime;
        }
        // Jumping Low...
        else if(vertical < 0.5f && rigidbody2d.velocity.y > 0)
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity.y * (fallShortMult - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collission) {
        if (collission.gameObject.tag == "Ground")
        {
            isGrounded = true;
            animator.SetBool("isOnGround", isGrounded);
        }
    }

    private void OnTriggerExit2D(Collider2D collission) {
        if (collission.gameObject.tag == "Ground")
        {
            isGrounded = false;
            animator.SetBool("isOnGround", isGrounded);
        }
    }

    private void Flip()
    {
        // Fliping all character...
        isFacingRight = !isFacingRight;
        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
        followCamera.flipMult *= -1;
        aiming.rotMult *= -1;
    }

    private void CheckRight(Joystick move, Joystick aim)
    {
        CheckRevers(move, aim);

        // Flip checking...
        if (!isFacingRight && aim.Horizontal > 0)
        {
            Flip();
        }
        else if (isFacingRight && aim.Horizontal < 0)
        {
            Flip();
        }
        else
        {
            if (!isFacingRight && move.Horizontal > 0 && aim.Horizontal == 0)
            {
                Flip();
            }
            else
            if (isFacingRight && move.Horizontal < 0 && aim.Horizontal == 0)
            {
                Flip();
            }
        }
    }

    private void CheckRevers(Joystick move, Joystick aim)
    {
        // Right side cheking for animation...
        if (move.Horizontal > 0 && aim.Horizontal < 0 || move.Horizontal < 0 && aim.Horizontal > 0)
        {
            animator.SetBool("isRevers", true);
        }
        else
        {
            animator.SetBool("isRevers", false);
        }
    }
}
