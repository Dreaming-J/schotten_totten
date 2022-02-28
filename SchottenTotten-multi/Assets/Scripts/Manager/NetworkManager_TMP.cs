using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager_TMP : MonoBehaviourPunCallbacks
{
    public Text StatusText;
    public Text TestText;
    bool isEntered = false;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        Application.targetFrameRate = 30;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (!isEntered && PhotonNetwork.PlayerList.Length == 2)
        {
            GameManager.Inst.StartGame();
            isEntered = true;
        }
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2, PublishUserId = true }, null);
}
