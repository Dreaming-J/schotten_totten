using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SyncManager : MonoBehaviourPunCallbacks
{
    public static SyncManager Inst { get; private set; }
    void Awake() => Inst = this;

    PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public void CallSyncitemList(List<Item> items)
    {
        PV.RPC("SyncItemList", RpcTarget.OthersBuffered, items);
    }

    [PunRPC] static void SyncItemList(List<Item> items)
    {
        CardManager.Inst.itemBuffer = items;
        Debug.Log("AAA");
    }
}
