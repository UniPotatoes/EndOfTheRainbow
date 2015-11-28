using UnityEngine;
using System.Collections;

public class DEBUG : MonoBehaviour {

	public Transform PivotPoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector2 (PivotPoint.transform.position.x, PivotPoint.transform.position.y);
	}
}
