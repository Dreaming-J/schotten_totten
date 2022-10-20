using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSO_Setting : MonoBehaviour
{
    [SerializeField] ItemSO itemSO;

    public static byte[,] colors = new byte[6, 3] { { 160, 204, 98 }, { 77, 202, 245 }, { 240, 99, 88 }, { 244, 204, 121 }, { 168, 110, 171 }, { 172, 132, 120 } };
    //public static string[] colorsname = new string[6] { "green", "blue", "red", "yellow", "purple", "brown" };

    void Start()
    {
        SetItemSO();
    }

    public void SetItemSO()
    {
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            Item item = itemSO.items[i];
            item.number = i / 6 + 1;
            item.colornum = i % 6;
            item.color = new Color32(colors[i % 6, 0], colors[i % 6, 1], colors[i % 6, 2], 255);
        }
    }
}