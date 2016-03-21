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
        target = GameObject.FindWithTag("Player").transform;
        maxCameraDisplacement = new Vector2(2.8f, 6);
    } 	

	void LateUpdate ()
    {
        transform.position = new Vector3(Mathf.Clamp(target.position.x, minCameraPosition.x, maxCameraPosition.x), 
                                         Mathf.Clamp(target.position.y, minCameraPosition.y, maxCameraPosition.y), -1);      
	}

    void ChangedRoom(Vector3 parametrs) //x-roomPosition.x; y-roomPosition.y; z-roomType  
    {
        MapDigger.RoomType roomType = (MapDigger.RoomType)parametrs.z;
        Vector2 roomPosition = new Vector2(parametrs.x, parametrs.y);

        minCameraPosition = new Vector2(roomPosition.x - maxCameraDisplacement.x, roomPosition.y - maxCameraDisplacement.y);
        maxCameraPosition = new Vector2(roomPosition.x + maxCameraDisplacement.x, roomPosition.y + maxCameraDisplacement.y);
        switch (roomType)
        {
            case MapDigger.RoomType.Room_1x2: { maxCameraPosition.x += 20; break; }
            case MapDigger.RoomType.Room_2x1: { maxCameraPosition.y += 20; break; }
            case MapDigger.RoomType.Room_2x2: { maxCameraPosition.x += 20; maxCameraPosition.y += 20; break; }
        }
    }
}
