using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    [SerializeField]
    private Vector2 maxCameraDisplacement;
    [SerializeField]
    private Vector2 minCameraPosition;
    [SerializeField]
    private Vector2 maxCameraPosition;

    private Transform target;

    void Start()
    {
        target = GameObject.Find("Player").transform;
    } 	

	void LateUpdate ()
    {
        transform.position = new Vector3(Mathf.Clamp(target.position.x, minCameraPosition.x, maxCameraPosition.x), 
                                         Mathf.Clamp(target.position.y, minCameraPosition.y, maxCameraPosition.y), -1);

        
	}
}
