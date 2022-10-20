using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class PRS //Position, Rotation, Scale을 담는 class
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}

public class Utils
{
    public static Quaternion QI => Quaternion.identity;
    public static Quaternion Q180 => Quaternion.Euler(0, 0, 180.0f); // 삭제 예정
    public static bool isMaster => PhotonNetwork.IsMasterClient;
    public static string ID => PhotonNetwork.LocalPlayer.UserId;
    public static WaitForSeconds delay(float num) { return new WaitForSeconds(num); }

    public static Vector3 MousePos
    {
        get
        {
            Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            result.z = -10;
            return result;
        }
    }

    public static int CompareCard(Card card1, Card card2)
    {
        if (CardManager.Inst.esortState == CardManager.ESortState.ColorNum)
        {
            if (card1.item.colornum < card2.item.colornum)
                return -1;
            else if (card1.item.colornum == card2.item.colornum)
            {
                if (card1.item.number < card2.item.number)
                    return -1;
                else if (card1.item.number > card2.item.number)
                    return 1;
            }
            else if (card1.item.colornum > card2.item.colornum)
                return 1;
        }
        else if (CardManager.Inst.esortState == CardManager.ESortState.NumColor)
        {
            if (card1.item.number < card2.item.number)
                return -1;
            else if (card1.item.number == card2.item.number)
            {
                if (card1.item.colornum < card2.item.colornum)
                    return -1;
                else if (card1.item.colornum > card2.item.colornum)
                    return 1;
            }
            else if (card1.item.number > card2.item.number)
                return 1;
        }

        return 0;
    }
}

[System.Serializable]
public class Serialization<T>
{
    public Serialization(List<T> _target) => target = _target;
    public List<T> target;
}

//포톤 오브젝트 생성
//PhotonNetwork.Instantiate("MyPrefabName", new Vector3(0, 0, 0), Quaternion.identity, 0);

//포톤 리스트 동기화
//string jdata = JsonUtility.ToJson(new Serialization<Item>(itemBuffer));
//PV.RPC(nameof(UpdateItemBuffer), RpcTarget.OthersBuffered, jdata);
//[PunRPC] void UpdateItemBuffer(string jdata)
//{
//    itemBuffer = JsonUtility.FromJson<Serialization<Item>>(jdata).target;
//}

//포톤 json 동기화
//string jdata = JsonUtility.ToJson(item);
//PV.RPC(nameof(SetupRPC), RpcTarget.AllBufferedViaServer, jdata);
//[PunRPC] public void SetupRPC(string jdata)
//{
//    item = JsonUtility.FromJson<Item>(jdata);
//}