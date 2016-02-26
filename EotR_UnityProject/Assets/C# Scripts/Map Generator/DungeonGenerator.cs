using UnityEngine;
using System.Collections;

public class DungeonGenerator : MonoBehaviour
{

    public int width;
    public int height;
    public int smoothSteps = 4;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    public int[,] map;

    public GameObject Rooms;
    public GameObject Room_1x1;
    public GameObject Room_1x2;
    public GameObject Room_2x1;
    public GameObject Room_2x2;

   // public GameObject Player;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        //r - resetowanie mapy, Debug
        if (Input.GetKey("r"))
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
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothSteps; i++)
        {
            SmoothMap();
        }

        PlaceRooms();

      //  SpawnPlayer();
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 0;
                }
                else {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    void PlaceRooms()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f, 0);
                       
                        GameObject tile = Instantiate(Room_1x1, pos, transform.rotation) as GameObject;
                        tile.transform.parent = Rooms.transform;        
                    }
                }
            }
        }
    }
    /*
    void SpawnPlayer()
    {
        int x; int y;
        bool foundPlace = false;
        for (x = 0; x < width; x++)
        {
            for (y = 0; y < height - 1; y++)
            {
                if (map[x, y] == 0 && map[x, y - 1] == 0)
                {
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y, 0);
                    Instantiate(Player, pos, transform.rotation);
                    foundPlace = true;
                    break;
                }
            }
            if (foundPlace == true)
            {
                break;
            }
        }
    }
    */
}
