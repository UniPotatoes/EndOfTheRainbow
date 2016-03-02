using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
	private GameObject player;
	private Rigidbody2D rigidBody;
	private float distance; //distance from player
	public float range = 10; //how far can it see
	public float speedPatroling = 1; //speed when not seeing a player
	public float speedTowardsPlayer = 3; //speed when seeing a player

	//wall checking
	public Transform wallCheckUR, wallCheckBR, wallCheckUL, wallCheckBL;
	[SerializeField]private bool wallOnRight;

	//ground checking
	public LayerMask whatIsGround; //define what is treated like ground
	/*float groundRadius = 0.05f;
	[SerializeField]private bool grounded = false;*/ //is it on the ground

	// Use this for initialization
	void Start () 
	{
		rigidBody = GetComponent<Rigidbody2D>();
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		CheckForWall ();
		distance = Vector2.Distance (this.gameObject.transform.position, player.transform.position);
		if (distance <= range)
			moveTowardsPlayer ();
		else
			moveOnPatrol ();
	}

	void moveTowardsPlayer()
	{
		Vector2 velocity = new Vector2 (speedTowardsPlayer, 0);
		if (this.gameObject.transform.position.x > player.transform.position.x) 
		{
			rigidBody.velocity = -velocity;
			wallOnRight = true;
		} 
		else 
		{
			rigidBody.velocity = velocity;
			wallOnRight = false;
		}
	}

	void moveOnPatrol()
	{
		if (!wallOnRight)
			rigidBody.velocity = new Vector2 (speedPatroling, 0);
		else if (wallOnRight)
			rigidBody.velocity = new Vector2 (-speedPatroling, 0);
	}

	void CheckForWall ()
	{
		Vector2 leftWall = new Vector2 (wallCheckBL.position.x, wallCheckBL.position.y);
		Vector2 rightWall = new Vector2 (wallCheckBR.position.x, wallCheckBR.position.y);
		if (Physics2D.OverlapArea (wallCheckUR.position, rightWall, whatIsGround) != null) 
		{
			wallOnRight = true;
		} 
		if(Physics2D.OverlapArea(wallCheckUL.position, leftWall, whatIsGround) != null)
		{
			wallOnRight = false;
		}
	}
}