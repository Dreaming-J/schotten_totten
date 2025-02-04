using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class User : MonoBehaviour
{
    [SerializeField] SpriteRenderer characterimg;
    [SerializeField] List<Sprite> sprites;
    [SerializeField] PhotonView PV;

    int id2mine => PV.IsMine ? 0 : 1;

    private void Start()
    {
        characterimg.sprite = sprites[id2mine];
        name = PV.IsMine ? "MyPlayer" : "OtherPlayer";
    }
}