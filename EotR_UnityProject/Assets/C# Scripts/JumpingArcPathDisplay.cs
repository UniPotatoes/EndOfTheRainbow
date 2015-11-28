using UnityEngine;
using System.Collections;

public class JumpingArcPathDisplay : MonoBehaviour {

	float mouseX, mouseY;
	[SerializeField]private float relativeMouseX, relativeMouseY;
	public int jumpForce = 1; //jump modifier
	public Transform PivotPoint;

	float jumpNowX, jumpNowY;
	private Rigidbody2D rigidBody;
	// Use this for initialization
	void Awake () {
		rigidBody = GetComponent<Rigidbody2D>();
		PivotPoint = GameObject.Find ("PivotPoint").transform;



		//stuff needed for jumping
		mouseX = Input.mousePosition.x; //mouse horizontal position on screen
		mouseY = Input.mousePosition.y; //mouse vertical position on screen
		relativeMouseX = mouseX - Screen.width / 2; //mouse horizontal position relative to center of the screen
		relativeMouseY = mouseY - Screen.height / 2; //mouse vertical position relative to center of the screen
		//float jumpX = relativeMouseX - PivotPoint.transform.position.x; //horizontal force modifiier to be applied to object
		//float jumpY = relativeMouseY - PivotPoint.transform.position.y; //vertical force modifiier to be applied to object
		float jumpX;
		if (relativeMouseX < -150.0f) {
			jumpX = -150.0f;
		} else {
			if (relativeMouseX > 150.0f) {
				jumpX = 150.0f;
			} else {
				jumpX = relativeMouseX;
			}
		}
		
		float jumpY;
		if (relativeMouseY > 200.0f) {
			jumpY = 200.0f;
		} else {
			jumpY = relativeMouseY;
		}
		
		jumpNowX = jumpForce * jumpX * 0.1f; //final horizontal jump force
		jumpNowY = jumpForce * jumpY * 0.1f; //final vertical jump force
		if (relativeMouseY > PivotPoint.transform.position.y + 5.0f) { //don't jump down, it's impossible
			Vector2 jumping = new Vector2 (jumpNowX, jumpNowY);
			rigidBody.AddForce (jumping, ForceMode2D.Impulse);
		}
	}
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.layer == 8 ){
			Debug.Log ("I've hit somethin");
			Destroy (gameObject);
		}
	}

}
