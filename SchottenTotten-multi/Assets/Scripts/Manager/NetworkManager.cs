using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> Prefabs;
    public Text StatusText;
    public Text TestText;
    bool isEntered = false;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        PreparePool();
    }

    private void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        TestText.text = "Master: " + Utils.isMaster.ToString() + "\nMyTurn: " + TurnManager.Inst.myTurn.ToString() + "\n³²Àº µ¦: " + CardManager.Inst.itemBuffer.Count.ToString() + "Àå";
        if (!isEntered && PhotonNetwork.PlayerList.Length == 2)
        {
            GameManager.Inst.StartGame();
            isEntered = true;
        }
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2, PublishUserId = true }, null);

    public override void OnJoinedRoom()
    {
    }

    void PreparePool()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && this.Prefabs != null)
        {
            foreach (GameObject prefab in this.Prefabs)
            {
                pool.ResourceCache.Add(prefab.name, prefab);
            }
        }
    }
}
