using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Pun;

public class FieldCard : MonoBehaviour
{
    [SerializeField] GameObject fieldcard;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text LT_NumberTMP;
    [SerializeField] TMP_Text RT_NumberTMP;
    [SerializeField] TMP_Text LB_NumberTMP;
    [SerializeField] TMP_Text RB_NumberTMP;

    public Item item;
    public PhotonView PV;

    public void Setup(Item item, int tilenum)
    {
        string jdata = JsonUtility.ToJson(item);
        PV.RPC(nameof(SetupRPC), RpcTarget.All, jdata, tilenum);
    }
    [PunRPC] public void SetupRPC(string jdata, int tilenum)
    {
        string tilename = PV.IsMine ? tilenum.ToString() : TileManager.TransTileName(tilenum).ToString();
        transform.parent = PV.IsMine ? GameObject.Find(tilename).transform.GetChild(0).transform : GameObject.Find(tilename).transform.GetChild(1).transform;
        item = JsonUtility.FromJson<Item>(jdata);
        int num = (item.number - 1) * 6;
        for (int j = 0; j < 6; j++)
        {
            if (item.colorname == CardManager.Inst.itemSO.items[num + j].colorname)
            {
                item.sprite = CardManager.Inst.itemSO.items[num + j].sprite;
                break;
            }
        }
        character.sprite = item.sprite;
        LT_NumberTMP.text = item.number.ToString();
        LT_NumberTMP.color = item.color;
        RT_NumberTMP.text = item.number.ToString();
        RT_NumberTMP.color = item.color;
        LB_NumberTMP.text = item.number.ToString();
        LB_NumberTMP.color = item.color;
        RB_NumberTMP.text = item.number.ToString();
        RB_NumberTMP.color = item.color;

        int order = transform.parent.GetComponent<Field>().child - 1; // 자식 컴포넌트가 추가된 후이므로
        GetComponent<Order>().SetOriginOrder(order);
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }

    void OnMouseOver()
    {
        FieldManager.Inst.FieldCardMouseOver(this);
    }

    void OnMouseExit()
    {
        FieldManager.Inst.FieldCardMouseExit(this);
    }
}
