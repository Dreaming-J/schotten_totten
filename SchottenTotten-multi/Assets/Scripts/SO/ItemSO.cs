using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int number;
    public Sprite sprite;
    public int colornum;
    public Color color;
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scrptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] items;
}