using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapDigger : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;

    public RoomType[,] map;

    public GameObject Rooms;
    public GameObject Room_1x1;
    public GameObject Room_1x2;
    public GameObject Room_2x1;
    public GameObject Room_2x2;
    public int totalNumberOfRooms = 4; // change if addedroom

    public int roomsToDig = 1;

    public enum RoomType {Occupied = -2, None, FreeSpace, Room_1x1, Room_1x2, Room_2x1, Room_2x2};

    public int minWidthOfMap = 0;

    public GameObject Player;
    public GameObject Camera;

    // Use this for initialization
    void Start()
    {
        if(minWidthOfMap == 0)
        {
            minWidthOfMap = (int)(mapWidth * 0.5f);
        }
        GenerateMap();
        SpawnPlayer();
    }

    // Update is called once per frame
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
        SpawnPlayer();
    }

    void GenerateMap()
    {
        InitialiseMap();

        DigPaths();
    }

    void InitialiseMap()
    {
        map = new RoomType[mapWidth, mapHeight];
        ResetMapValues();
    }

    void ResetMapValues()
    {
        for(int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
                map[i, j] = RoomType.None;
    }

    enum Direction { North, South, West, East };

    void DigPaths()
    {
        List<Direction> dugPath = new List<Direction>(); //stworzenie listy na przechowywanie sciezki ruchu diggera
        bool mapWideEnough = false;
        while (!mapWideEnough)
        {
            dugPath.Clear();
            Vector2 diggerPosition = new Vector2(0, mapHeight / 2); //pozycja startowa diggera
            ResetMapValues();
            map[(int)diggerPosition.x, (int)diggerPosition.y] = RoomType.FreeSpace;
        
            int roomsDug = 1;
            while (roomsDug < roomsToDig)
            {
                Direction direction = (Direction)Random.Range(0, 4);
                dugPath.Add(direction); //dodaj kolejny ruch do listy
                diggerPosition = MoveDiggerInDirection(diggerPosition, direction);
                if (map[(int)diggerPosition.x, (int)diggerPosition.y] != RoomType.FreeSpace)
                {
                    map[(int)diggerPosition.x, (int)diggerPosition.y] = RoomType.FreeSpace;
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
                if (map[i, j] == RoomType.FreeSpace)
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
        return diggerPosition += diggerMove;
    }

    void PlaceRoomsOnMap()
    {
        if (map != null)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (map[x, y] == RoomType.FreeSpace)
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
        avaliableRooms = new bool[totalNumberOfRooms + 1];
        for (int i = (int)RoomType.Room_1x1; i < totalNumberOfRooms + 1; i++) avaliableRooms[i] = true;
        // choose the room
        if (x == mapWidth - 1 || map[x + 1, y] != RoomType.FreeSpace) avaliableRooms[(int)RoomType.Room_1x2] = false;
        if (y == mapHeight - 1 || map[x, y + 1] != RoomType.FreeSpace) avaliableRooms[(int)RoomType.Room_2x1] = false;
        if (x == mapWidth - 1 || y == mapHeight - 1 || map[x + 1, y + 1] != RoomType.FreeSpace || map[x + 1, y] != RoomType.FreeSpace || map[x, y + 1] != RoomType.FreeSpace) avaliableRooms[(int)RoomType.Room_2x2] = false;

        int numberOfAvaliableRooms = 1;
        for (int i = (int)RoomType.Room_1x1; i < totalNumberOfRooms + 1; i++) if (avaliableRooms[i] == true) numberOfAvaliableRooms++;
        int numberToCountProbability = 6; if (numberOfAvaliableRooms != totalNumberOfRooms) numberToCountProbability = numberOfAvaliableRooms;


        RoomType choosedRoom = RoomType.FreeSpace;
        while ((int)choosedRoom > totalNumberOfRooms || avaliableRooms[(int)choosedRoom] == false)
        {
            choosedRoom = (RoomType)Random.Range(1, numberToCountProbability);
        }
        //place choosed room
        switch (choosedRoom)
        {
            case RoomType.Room_1x1:
                {
                    map[x, y] = RoomType.Room_1x1;
                    PlaceRoomOnPosition(x, y, RoomType.Room_1x1);
                    //Debug.Log("1x1");
                    break;
                }
            case RoomType.Room_1x2:
                {
                    map[x, y] = RoomType.Room_1x2;
                    PlaceRoomOnPosition(x, y, RoomType.Room_1x2);
                    map[x + 1, y] = RoomType.Occupied;
                    //Debug.Log("1x2");
                    break;
                }
            case RoomType.Room_2x1:
                {
                    map[x, y] = RoomType.Room_2x1;
                    PlaceRoomOnPosition(x, y, RoomType.Room_2x1);
                    map[x, y + 1] = RoomType.Occupied;
                    //Debug.Log("2x1");
                    break;
                }
            case RoomType.Room_2x2:
                {
                    map[x, y] = RoomType.Room_2x2;
                    PlaceRoomOnPosition(x, y, RoomType.Room_2x2);
                    map[x + 1, y] = map[x, y + 1] = map[x + 1, y + 1] = RoomType.Occupied;
                    //Debug.Log("2x2");
                    break;
                }
            default:
                Debug.Log("Room not choosen");
                break;
        }
    }

    void PlaceRoomOnPosition(float x, float y, RoomType roomType)
    {
        //Vector3 pos = new Vector3(-mapWidth / 2 + x + .5f, -mapHeight / 2 + y + .5f, 0); //Wspolrzedne obliczane tak, by srodek mapy byl w srodku ukladu wspolrzednych
        Vector3 pos = new Vector3(20*x+10,20*y+10,0);
        GameObject room = Room_1x1;

        switch (roomType)
        {
            case RoomType.Room_1x2:
                {
                    room = Room_1x2;
                    break;
                }
            case RoomType.Room_2x1:
                {
                    room = Room_2x1;
                    break;
                }
            case RoomType.Room_2x2:
                {
                    room = Room_2x2;
                    break;
                }

        }
        GameObject tile = Instantiate(room, pos, transform.rotation) as GameObject;
        tile.transform.parent = Rooms.transform;
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
                        if (doorBreakerPosition.y < mapHeight - 1)
                        {
                            doorBreakerPosition = MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(doorBreakerPosition, Vector2.up);  
                        }
                        break;
                    }
                case Direction.South:
                    {
                        if (doorBreakerPosition.y > 0)
                        {
                            doorBreakerPosition = MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(doorBreakerPosition, Vector2.down);
                        }
                        break;
                    }
                case Direction.West:
                    {
                        if (doorBreakerPosition.x > 0)
                        {
                            doorBreakerPosition = MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(doorBreakerPosition, Vector2.left);
                        }
                        break;
                    }
                case Direction.East:
                    {
                        if (doorBreakerPosition.x < mapWidth - 1)
                        {
                            doorBreakerPosition = MoveDoorBreakerInDirectionAndDestroyAllEntranceObjectsOnItsWay(doorBreakerPosition, Vector2.right);
                        }
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
        Vector3 pos = new Vector3(10, 1020, 0);
        Instantiate(Player, pos, transform.rotation);
        Instantiate(Camera, pos, transform.rotation);
    }


    public RoomType GetRoomTypeOnPosition(Vector2 coordinatesOfRoom)
    {
        return map[(int)coordinatesOfRoom.x, (int)coordinatesOfRoom.y];
    }
}
