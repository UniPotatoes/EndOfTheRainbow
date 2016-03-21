using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapDigger : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;

    public Room.Type[,] map;
    
    public List<Room> roomList;

    public int roomsToDig = 1;

    public int minWidthOfMap = 0;

    GameObject RoomContainer;
    public GameObject Player;
    public GameObject Camera;

    void Start()
    {
        RoomContainer = new GameObject(); RoomContainer.transform.position = Vector3.zero; RoomContainer.name = "Rooms";
        
        if (minWidthOfMap == 0)
        {
            minWidthOfMap = (int)(mapWidth * 0.5f);
        }
        GenerateMap();
        SpawnPlayer();
    }

    void Update()
    {
        //r - resetowanie mapy, Debug
        if (Input.GetKeyDown("r"))
        {
            ResetMap();
        }
    }

    void ResetMap()
    {
        var gameObjects = GameObject.FindGameObjectsWithTag("Room");
        foreach (var target in gameObjects)
        {
            GameObject.Destroy(target);
        }

        GenerateMap();
    }

    void GenerateMap()
    {
        InitialiseMap();

        DigPaths();
    }

    void InitialiseMap()
    {
        map = new Room.Type[mapWidth, mapHeight];
        ResetMapValues();
    }

    void ResetMapValues()
    {
        for(int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
                map[i, j] = Room.Type.None;
    }

    enum Direction { North, South, West, East };

    void DigPaths()
    {
        Vector2 diggerMove;
        List<Direction> dugPath = new List<Direction>(); //stworzenie listy na przechowywanie sciezki ruchu diggera
        bool mapWideEnough = false;
        while (!mapWideEnough)
        {
            dugPath.Clear();
            Vector2 diggerPosition = new Vector2(0, mapHeight / 2); //pozycja startowa diggera
            ResetMapValues();
            map[(int)diggerPosition.x, (int)diggerPosition.y] = Room.Type.FreeSpace;
        
            int roomsDug = 1;
            while (roomsDug < roomsToDig)
            {
                Direction direction = (Direction)Random.Range(0, 4);
                
                diggerMove = MoveDiggerInDirection(diggerPosition, direction);
                if (diggerMove != Vector2.zero)
                {
                    dugPath.Add(direction); //dodaj kolejny ruch do listy
                    diggerPosition += diggerMove;
                }               
                if (map[(int)diggerPosition.x, (int)diggerPosition.y] != Room.Type.FreeSpace)
                {
                    map[(int)diggerPosition.x, (int)diggerPosition.y] = Room.Type.FreeSpace;
                    roomsDug += 1;
                }
            }
            mapWideEnough = CheckIfMapIsWiderThan(minWidthOfMap);
        }

        PlaceRoomsOnMap();

        DeleteEntranceBlocksUsingRayCast(dugPath);
    }

    bool CheckIfMapIsWiderThan(int minWidthOfMap)
    {
        int indexOfMostRightRoom = 0;
        for (int j = 0; j < mapHeight; j++)
        {
            for (int i = mapWidth-1; i > 0; i--)
            {
                if (map[i, j] == Room.Type.FreeSpace)
                {
                    indexOfMostRightRoom = i;
                    break;
                }
            }
            if(indexOfMostRightRoom != 0)
            {
                break;
            }
        }
        return indexOfMostRightRoom >= minWidthOfMap;
    }

    Vector2 MoveDiggerInDirection(Vector2 diggerPosition, Direction direction)
    {
        Vector2 diggerMove = Vector2.zero;
        switch (direction)
        {
            case Direction.North: { if (diggerPosition.y < mapHeight - 1) diggerMove = Vector2.up; break; }
            case Direction.South: { if (diggerPosition.y > 0) diggerMove = Vector2.down; break; }
            case Direction.West: { if (diggerPosition.x > 0) diggerMove = Vector2.left; break; }
            case Direction.East: { if (diggerPosition.x < mapWidth - 1) diggerMove = Vector2.right; break; }
        }
        return diggerMove;
    }

    void PlaceRoomsOnMap()
    {
        if (map != null)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (map[x, y] == Room.Type.FreeSpace)
                    {
                        ChooseRoomToPlace(x, y);
                    }
                }
            }
        }
    }

    void ChooseRoomToPlace(int x, int y)
    {
        bool[] avaliableRooms;
        avaliableRooms = new bool[roomList.Count + 1];
        for (int i = (int)Room.Type._1x1; i < roomList.Count + 1; i++) avaliableRooms[i] = true;
        // choose the room
        if (x == mapWidth - 1 || map[x + 1, y] != Room.Type.FreeSpace) avaliableRooms[(int)Room.Type._1x2] = false;
        if (y == mapHeight - 1 || map[x, y + 1] != Room.Type.FreeSpace) avaliableRooms[(int)Room.Type._2x1] = false;
        if (x == mapWidth - 1 || y == mapHeight - 1 || map[x + 1, y + 1] != Room.Type.FreeSpace || map[x + 1, y] != Room.Type.FreeSpace || map[x, y + 1] != Room.Type.FreeSpace) avaliableRooms[(int)Room.Type._2x2] = false;

        int numberOfAvaliableRooms = 1;
        for (int i = (int)Room.Type._1x1; i < roomList.Count + 1; i++) if (avaliableRooms[i] == true) numberOfAvaliableRooms++;
        int numberToCountProbability = 6; if (numberOfAvaliableRooms != roomList.Count) numberToCountProbability = numberOfAvaliableRooms;


        Room.Type choosedRoom = Room.Type.FreeSpace;
        while ((int)choosedRoom > roomList.Count || avaliableRooms[(int)choosedRoom] == false)
        {
            choosedRoom = (Room.Type)Random.Range(1, numberToCountProbability);
        }
        //place choosed room
        switch (choosedRoom)
        {
            case Room.Type._1x1:
                {
                    map[x, y] = Room.Type._1x1;
                    PlaceRoomOnPosition(x, y, Room.Type._1x1);
                    break;
                }
            case Room.Type._1x2:
                {
                    map[x, y] = Room.Type._1x2;
                    PlaceRoomOnPosition(x, y, Room.Type._1x2);
                    map[x + 1, y] = Room.Type.Occupied;
                    break;
                }
            case Room.Type._2x1:
                {
                    map[x, y] = Room.Type._2x1;
                    PlaceRoomOnPosition(x, y, Room.Type._2x1);
                    map[x, y + 1] = Room.Type.Occupied;
                    break;
                }
            case Room.Type._2x2:
                {
                    map[x, y] = Room.Type._2x2;
                    PlaceRoomOnPosition(x, y, Room.Type._2x2);
                    map[x + 1, y] = map[x, y + 1] = map[x + 1, y + 1] = Room.Type.Occupied;
                    break;
                }
            default:
                Debug.Log("Room not choosen");
                break;
        }
    }

    void PlaceRoomOnPosition(float x, float y, Room.Type roomType)
    {
        Vector3 pos = new Vector3(20*x+10,20*y+10,0);
        GameObject room = roomList[0].room;

        switch (roomType)
        {
            case Room.Type._1x2:
                {
                    room = roomList[1].room;
                    break;
                }
            case Room.Type._2x1:
                {
                    room = roomList[2].room;
                    break;
                }
            case Room.Type._2x2:
                {
                    room = roomList[3].room;
                    break;
                }
        }
        GameObject tile = Instantiate(room, pos, transform.rotation) as GameObject;
        tile.transform.parent = RoomContainer.transform;
        tile.transform.name = roomType.ToString() + "_" + x + "," + y;
    }

    void DeleteEntranceBlocksUsingRayCast(List<Direction> dugPath)
    {
        Vector2 doorBreakerPosition = new Vector2(0, mapHeight / 2);
        
        foreach (Direction element in dugPath)
        {
            switch (element)
            {
                case Direction.North:
                    {
                        doorBreakerPosition = MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(doorBreakerPosition, Vector2.up);  
                        break;
                    }
                case Direction.South:
                    {
                        doorBreakerPosition = MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(doorBreakerPosition, Vector2.down);
                        break;
                    }
                case Direction.West:
                    {
                        doorBreakerPosition = MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(doorBreakerPosition, Vector2.left);
                        break;
                    }
                case Direction.East:
                    {
                        doorBreakerPosition = MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(doorBreakerPosition, Vector2.right);
                        break;
                    }
            }
        }
    }

    Vector2 MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(Vector2 doorBreakerPosition, Vector2 direction)
    {
        Vector2 pos = new Vector2(doorBreakerPosition.x * 20 + 10, doorBreakerPosition.y * 20 + 10);
        RaycastHit2D[] hit = Physics2D.RaycastAll(pos, direction, 20.0f);
        DestroyObjectHitWithRaycast(hit);
        return doorBreakerPosition + direction;
    }

    void DestroyObjectHitWithRaycast(RaycastHit2D[] hit)
    {
        foreach (RaycastHit2D element in hit)
        {
            if (element.collider.CompareTag("Entrance"))
            {
                Destroy(element.collider.gameObject);
            }
        }
    }

    void SpawnPlayer()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Instantiate(Player, pos, transform.rotation);
        Instantiate(Camera, pos, transform.rotation);
        Player.transform.position = new Vector3(100, 1010, 0);
    }

    public Room.Type GetRoomTypeOnPosition(Vector2 coordinatesOfRoom)
    {
        return map[(int)coordinatesOfRoom.x, (int)coordinatesOfRoom.y];
    }
}
