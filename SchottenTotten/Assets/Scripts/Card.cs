using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

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
    public PRS originPRS;

    public void Setup(Item item, bool isFront)
    {
        this.item = item;
        this.isFront = isFront;

        if (this.isFront)
        {
            character.sprite = this.item.sprite;
            LT_NumberTMP.text = this.item.number.ToString();
            LT_NumberTMP.color = this.item.color;
            RT_NumberTMP.text = this.item.number.ToString();
            RT_NumberTMP.color = this.item.color;
            LB_NumberTMP.text = this.item.number.ToString();
            LB_NumberTMP.color = this.item.color;
            RB_NumberTMP.text = this.item.number.ToString();
            RB_NumberTMP.color = this.item.color;
        }
        else
        {
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
