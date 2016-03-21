using UnityEngine;
using System.Collections;

public class RoomChanging : MonoBehaviour {

    public GameObject mainCamera;
    public GameObject mapGenerator;
    
    void Start()
    {
        mainCamera = GameObject.FindWithTag("Camera");
        mapGenerator = GameObject.FindWithTag("MapGenerator");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            Vector2 coordinatesOfRoom = new Vector2( (transform.position.x - 10) / 20.0f, (transform.position.y - 10) / 20.0f );
            Debug.Log((int)mapGenerator.GetComponent<MapDigger>().GetRoomTypeOnPosition(coordinatesOfRoom));
            Vector3 parametrs = new Vector3(transform.position.x, transform.position.y, (int)mapGenerator.GetComponent<MapDigger>().GetRoomTypeOnPosition(coordinatesOfRoom));
            
            mainCamera.gameObject.SendMessage("ChangedRoom", parametrs, SendMessageOptions.RequireReceiver);
        }
    }
}
