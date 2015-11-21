using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
	//--------------------
	//	Debuggin
	public float velocityX;
	public float velocityY;
	//-----------------


    //moving
    public float horizontalSpeed = 10f; //horizontal movement speed
    
    //wall checking
    public Transform wallCheckUR, wallCheckBR, wallCheckUL, wallCheckBL;
    [SerializeField]private bool walled;

    //ground checking
    public Transform groundCheck; //groundCheck object
    public LayerMask whatIsGround; //define what is treated like ground
    float groundRadius = 0.1f;
    [SerializeField]private bool grounded = false; //is it on the ground

    //jumping
    public int jumpForce = 2; //jump modifier
    public int maxJumpForce = 700; //maximum jump force (not working now)
    public float mouseX, mouseY; //mouse coordinates on screen
    float jumpNowY;
    float jumpNowX;
    private Rigidbody2D rigidBody; //rigidbody which will jump
    [SerializeField]private float relativeMouseX, relativeMouseY; //mouse coordinates relative to the object

    //facing
    public bool facingRight = true;
    public bool previousFacingRight = true;
    Animator anim;

    void Start ()
    {
		rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}

	void Update ()
    {
        //--------------------
        //	Debuggin
        velocityX = rigidBody.velocity.x;
        velocityY = rigidBody.velocity.y;
	    //-----------------

        if (grounded && Input.GetKeyDown (KeyCode.Mouse0)) //if it is on the ground then jump when key is pressed
        {
            anim.SetTrigger("jumped");
            Jump();
        }
		
		if (grounded)
        {
			anim.SetBool ("onLand", true);
		}
        else
            {
                anim.SetBool("onLand", false);          
            }

        // flipping and animation control
        if (rigidBody.velocity.x < 0)
        {

            facingRight = false;
            anim.SetBool("walking", true);
            if (previousFacingRight == true)
            {
                Flip();
            }
            previousFacingRight = facingRight;
        }
        else
        {
            if (rigidBody.velocity.x > 0)
            {
                facingRight = true;
                anim.SetBool("walking", true);
                if (previousFacingRight == false)
                {
                    Flip();
                }
                previousFacingRight = facingRight;
            }
            else
            {
                anim.SetBool("walking", false);
            }
        }		
	}
    void FixedUpdate()
    {
		//checks if GroundCheck object is colliding with something
		grounded = CheckForGround ();
		if (grounded) //Horizontal movement
		{
			Move();
		}

		//checks if any of WallCheck objects is colliding with something
		walled = CheckForWall ();
		if (walled)
        {
			walled = false;
			rigidBody.velocity = new Vector2(-rigidBody.velocity.x, rigidBody.velocity.y);
        }
    }

    bool CheckForGround ()
	{
		if (Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround) != null) 
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}

	bool CheckForWall ()
	{
		if (Physics2D.OverlapArea (wallCheckUR.position, wallCheckBR.position, whatIsGround) != null) 
		{
			return true;
		} 
		else 
		{
			if(Physics2D.OverlapArea(wallCheckUL.position, wallCheckBL.position, whatIsGround) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

    void Jump()
    {
        //stuff needed for jumping
        mouseX = Input.mousePosition.x; //mouse horizontal position on screen
        mouseY = Input.mousePosition.y; //mouse vertical position on screen
        relativeMouseX = mouseX - Screen.width / 2; //mouse horizontal position relative to center of the screen
        relativeMouseY = mouseY - Screen.height / 2; //mouse vertical position relative to center of the screen
        float jumpX = relativeMouseX - transform.position.x; //horizontal force modifiier to be applied to object
        float jumpY = relativeMouseY - transform.position.y; //vertical force modifiier to be applied to object
        jumpNowX = jumpForce * jumpX; //final horizontal jump force
        jumpNowY = jumpForce * jumpY; //final vertical jump force
        if (relativeMouseY > transform.position.y) //don't jump down, it's impossible
        {
            Vector2 jumping = new Vector2 (jumpNowX, jumpNowY);
            rigidBody.AddForce(jumping, ForceMode2D.Force);
        }
    }

    void Move()
    {
        float move = Input.GetAxis("Horizontal"); //poruszanie się w poziomie
        rigidBody.velocity = new Vector2(move * horizontalSpeed, rigidBody.velocity.y);
    }

    void Flip()
    {
        //facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale; 
    }
}