using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Pun;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text LT_NumberTMP;
    [SerializeField] TMP_Text RT_NumberTMP;
    [SerializeField] TMP_Text LB_NumberTMP;
    [SerializeField] TMP_Text RB_NumberTMP;
    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;

    public Item item;
    bool isFront;
    public bool isField;
    public PRS originPRS;
    public PhotonView PV;

    private void Start()
    {
        isFront = PV.IsMine;
    }

    public void Setup(Item item)
    {
        //통신 지연이 있어 이부분만 미리 처리
        this.item.number = item.number;
        this.item.colornum = item.colornum;

        string jdata = JsonUtility.ToJson(item);
        PV.RPC(nameof(SetupRPC), RpcTarget.AllBufferedViaServer, jdata);
    }
    [PunRPC] public void SetupRPC(string jdata)
    {
        item = JsonUtility.FromJson<Item>(jdata);

        if (isFront)
        {
            card.sprite = cardFront;
            character.sprite = item.sprite;
            LT_NumberTMP.text = item.number.ToString();
            LT_NumberTMP.color = item.color;
            RT_NumberTMP.text = item.number.ToString();
            RT_NumberTMP.color = item.color;
            LB_NumberTMP.text = item.number.ToString();
            LB_NumberTMP.color = item.color;
            RB_NumberTMP.text = item.number.ToString();
            RB_NumberTMP.color = item.color;
        }
        else
        {
            transform.parent = CardManager.Inst.otherHand.transform;
            card.sprite = cardBack;
            LT_NumberTMP.text = "";
            RT_NumberTMP.text = "";
            LB_NumberTMP.text = "";
            RB_NumberTMP.text = "";
        }
    }

    void OnMouseOver()
    {
        if (isFront)
            CardManager.Inst.CardMouseOver(this);
    }

    void OnMouseExit()
    {
        if (isFront)
            CardManager.Inst.CardMouseExit(this);
    }

    void OnMouseDown()
    {
        if (isFront)
            CardManager.Inst.CardMouseDown();
    }

    void OnMouseUp()
    {
        if (isFront)
            CardManager.Inst.CardMouseUp();
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
}
