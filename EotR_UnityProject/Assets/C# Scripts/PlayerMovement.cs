using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	//moving
	public float horizontalSpeed = 10f; //horizontal movement speed
	
	//wall checking
	public Transform wallCheckUR, wallCheckBR, wallCheckUL, wallCheckBL;
	public Transform PivotPoint;
	[SerializeField]private bool leftWalled = false;
    [SerializeField]private bool rightWalled = false;

    //ground checking
    public LayerMask whatIsGround; //define what is treated like ground
	float groundRadius = 0.05f;
	[SerializeField]private bool grounded = false; //is it on the ground
	
	//jumping
	private Vector2 jumpingForce;
	public float jumpForce = 1.0f; //jump modifier
	public float jumpModifier = 5.0f;
	private Rigidbody2D rigidBody; //rigidbody which will jump
	
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
		if (grounded && Input.GetKeyDown(KeyCode.W)) //if it is on the ground then jump when key is pressed
		{
			anim.SetTrigger("jumped");
			Jump();
		}
		/*
		if (Input.GetKey (KeyCode.W)) 
		{
			if(jumpingForce.y > 0.0f)
			{
				jumpingForce.y += jumpModifier * Time.deltaTime;
			}
		}

		if (Input.GetKeyUp (KeyCode.W)) 
		{
			jumpForce = 0.0f;
		}
*/
		if (grounded)
		{
	//		jumpingForce.y = 0.0f;
			anim.SetBool ("onLand", true);
		}
		else
		{
			anim.SetBool("onLand", false);          
		}
		
		// flipping and animation control
		if (rigidBody.velocity.x < 0 /*&& !walled*/)
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
			if (rigidBody.velocity.x > 0 /*&& !walled*/)
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

		Move();
		leftWalled = CheckForLeftWall ();
        rightWalled = CheckForRightWall();
	}
	
	bool CheckForGround ()
	{
		Vector2 positionBL = wallCheckBL.position;
		Vector2 positionBR = new Vector2 (wallCheckBR.position.x, wallCheckBR.position.y - groundRadius);
		if (Physics2D.OverlapArea (positionBL, positionBR, whatIsGround) != null) 
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}
	
	bool CheckForLeftWall ()
	{
		Vector2 leftWall = new Vector2 (wallCheckBL.position.x, wallCheckBL.position.y + 0.03f);
		
		if(Physics2D.OverlapArea(wallCheckUL.position, leftWall, whatIsGround) != null)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

    bool CheckForRightWall()
    {
        Vector2 rightWall = new Vector2(wallCheckBR.position.x, wallCheckBR.position.y + 0.03f);
        if (Physics2D.OverlapArea(wallCheckUR.position, rightWall, whatIsGround) != null)
        {
            return true;
        }
        else
        {

            return false;          
        }
    }

    void Jump()
	{
		jumpingForce.x = 0.0f;
		jumpingForce.y = jumpForce;
		rigidBody.AddForce(jumpingForce, ForceMode2D.Impulse);
	}
	
	void Move()
	{
		float move = Input.GetAxis ("Horizontal") * horizontalSpeed; //poruszanie się w poziomie

        if (move > 0 && !rightWalled)
        {
            rigidBody.velocity = new Vector2(move, rigidBody.velocity.y);
        }
        else
        {
            if (move < 0 && !leftWalled)
            {
                rigidBody.velocity = new Vector2(move, rigidBody.velocity.y);
            }
        }	
	}
	
	void Flip()
	{
		//facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale; 
	}
}


















