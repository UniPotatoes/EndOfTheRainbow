using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{

	public GameObject JumpingArcDot;
	float interval = 0.01f; 
	float nextTime = 0;
	
	//-----------------
    //moving
    public float horizontalSpeed = 10f; //horizontal movement speed
    
    //wall checking
    public Transform wallCheckUR, wallCheckBR, wallCheckUL, wallCheckBL;
	public Transform PivotPoint;
    [SerializeField]private bool walled;

    //ground checking
    public LayerMask whatIsGround; //define what is treated like ground
    float groundRadius = 0.05f;
    [SerializeField]private bool grounded = false; //is it on the ground

    //jumping
    public int jumpForce = 1; //jump modifier
    public int maxJumpForce = 700; //maximum jump force (not working now)
    float mouseX, mouseY; //mouse coordinates on screen
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
		if(Input.GetKey((KeyCode.Space)))
		if (Time.time >= nextTime ) {
			nextTime += interval; 
			Debug.Log(nextTime);

			SpawnJumpingArcDot ();
		}


		mouseX = Input.mousePosition.x; //mouse horizontal position on screen
		mouseY = Input.mousePosition.y; //mouse vertical position on screen
		relativeMouseX = mouseX - Screen.width / 2; //mouse horizontal position relative to center of the screen
		relativeMouseY = mouseY - Screen.height / 2; //mouse vertical position relative to center of the screen


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
        if (rigidBody.velocity.x < 0 && !walled)
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
			if (rigidBody.velocity.x > 0 && !walled)
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
		if (grounded /*&& anim.GetBool("onLand") == true*/) //Horizontal movement
		{
			Move();
		}

		walled = CheckForWall ();
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
		//float jumpX = relativeMouseX - PivotPoint.transform.position.x; //horizontal force modifiier to be applied to object
		//float jumpY = relativeMouseY - PivotPoint.transform.position.y; //vertical force modifiier to be applied to object
		float jumpX;
		if (relativeMouseX < -150.0f) 
		{
			jumpX = -150.0f;
		} 
		else 
		{
			if (relativeMouseX > 150.0f) 
			{
				jumpX = 150.0f;
			}
			else
			{
			jumpX = relativeMouseX;
			}
		}

		float jumpY;
		if (relativeMouseY > 200.0f) 
		{
			jumpY = 200.0f;
		} 
		else 
		{
			jumpY = relativeMouseY;
		}

		jumpNowX = jumpForce * jumpX * 0.1f; //final horizontal jump force
		jumpNowY = jumpForce * jumpY * 0.1f; //final vertical jump force
		if (relativeMouseY > PivotPoint.transform.position.y + 5.0f) //don't jump down, it's impossible
        {
            Vector2 jumping = new Vector2 (jumpNowX, jumpNowY);
            rigidBody.AddForce(jumping, ForceMode2D.Impulse);
        }
    }

    void Move()
    {
		float move;
		if (rigidBody.velocity.y == 0) {
			move = Input.GetAxis ("Horizontal") * horizontalSpeed; //poruszanie się w poziomie
		} 
		else 
		{
			move = rigidBody.velocity.x;
		}
        rigidBody.velocity = new Vector2(move, rigidBody.velocity.y);
    }

    void Flip()
    {
        //facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale; 
    }

	void SpawnJumpingArcDot ()
	{
		Vector3 InstantiatePoint = new Vector3 (PivotPoint.transform.position.x, PivotPoint.transform.position.y, PivotPoint.transform.position.z);
		Instantiate (JumpingArcDot, InstantiatePoint, Quaternion.identity);

	}
}


















