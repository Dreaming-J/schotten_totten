using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string number;
    public Sprite sprite;
    public string colorname;
    public Color color;
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scrptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] items;
}