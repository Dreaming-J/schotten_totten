using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SyncOptions : MonoBehaviour, IPunObservable
{
    [Header("Sync Options")]
    [SerializeField] EPosSyncMode ePosSyncMode;
    [SerializeField] ERotSyncMode eRotSyncMode;

    enum EPosSyncMode { None, XaxisSym, YaxisSym, Origin }
    enum ERotSyncMode { Rot0, Rot180 }
    PhotonView PV;
    Vector3 pos;
    Quaternion rot;
    float Xpos;
    float Ypos;
    float Zrot;

    #region µø±‚»≠ IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(Xpos);
            stream.SendNext(Ypos);
        }
        else
        {
            //Network player, receive data
            Xpos = (float)stream.ReceiveNext();
            Ypos = (float)stream.ReceiveNext();
            UpdateSyncPos();
        }
    }
    #endregion

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (eRotSyncMode == ERotSyncMode.Rot180) UpdateSyncRot();
    }

    void Update()
    {
        UpdateMyPos();
    }

    public void UpdateSyncPos()
    {
        if (ePosSyncMode == EPosSyncMode.Origin)
        {
            Xpos = -1 * Xpos;
            Ypos = -1 * Ypos;
        }
        else if (ePosSyncMode == EPosSyncMode.XaxisSym)
            Ypos = -1 * Ypos;
        else if (ePosSyncMode == EPosSyncMode.YaxisSym)
            Xpos = -1 * Xpos;
        pos[0] = Xpos;
        pos[1] = Ypos;
        transform.position = pos;
    }

    void UpdateMyPos()
    {
        if (!PV.IsMine)
            return;

        pos = transform.position;
        Xpos = pos[0];
        Ypos = pos[1];
    }

    void UpdateSyncRot()
    {
        if (PV.IsMine)
            return;

        rot = transform.rotation;
        Zrot = rot[2] + 180;
        rot[2] = Zrot;
        transform.rotation = rot;
    }
}