using UnityEngine;
using System.Collections;

[System.Serializable]
public class Room
{
    public GameObject room;
    public int maxNumberOfOccouring;
    public Type type;

    [System.Serializable]
    public enum Type
    {
        Occupied = -2,
        None,
        FreeSpace,
        _1x1,
        _1x2,
        _2x1,
        _2x2
    }

    public Room(GameObject obiect, int number)
    {
        room = obiect;
        maxNumberOfOccouring = number;
    }

    public Room()
    {
    }
}