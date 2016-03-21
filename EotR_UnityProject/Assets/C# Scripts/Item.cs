using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item
{

    public string itemName;
    public int itemID;

    [System.Serializable]
    public enum ItemType
    {
        Mammal,
        Bird,
        Fish,
        Insect,
        Reptile
    }

    public Item(string name, int id)
    {
        itemName = name;
        itemID = id;

    }

    public Item()
    {
    }
}