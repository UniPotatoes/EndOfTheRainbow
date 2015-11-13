using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    public float VerticalSpeed = 10f; //vertical movement speed

    [SerializeField]private bool grounded = false; //is it on the ground
    public Transform groundCheck; //groundCheck object
    float groundRadius = 0.2f;
    public LayerMask whatIsGround; //define what is treated like ground
    public int jumpForce = 2; //jump modifier
    public int maxJumpForce = 700; //maximum jump force (not working now)

    private Rigidbody2D rigidBody; //rigidbody which will jump
    float jumpNowY;
    float jumpNowX;
    public float mouseX, mouseY; //mouse coordinates on screen
    [SerializeField]private float relativeMouseX, relativeMouseY; //mouse coordinates relative to the object


    void Start ()
    {
		rigidBody = GetComponent<Rigidbody2D>();
	}

	void Update ()
    {
        if(grounded && Input.GetKeyDown(KeyCode.Mouse0)) //if it is on the ground then jump when key is pressed
        {
            Jump();
        }
	
	}
    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround); //checks if GroundCheck object is colliding with something

        if (grounded) //Horizontal movement
        {
            Move();
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
        rigidBody.velocity = new Vector2(move * VerticalSpeed, rigidBody.velocity.y);
    }
}