using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "TileSO", menuName = "Scrptable Object/TileSO")]
public class TileSO : ScriptableObject
{
    public Tile[] tiles;
}