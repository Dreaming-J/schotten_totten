using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FieldCard : MonoBehaviour
{
    [SerializeField] GameObject fieldcard;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text LT_NumberTMP;
    [SerializeField] TMP_Text RT_NumberTMP;
    [SerializeField] TMP_Text LB_NumberTMP;
    [SerializeField] TMP_Text RB_NumberTMP;

    public Item item;

    public void Setup(Item item)
    {
        this.item = item;
        character.sprite = this.item.sprite;
        LT_NumberTMP.text = this.item.number.ToString();
        LT_NumberTMP.color = this.item.color;
        RT_NumberTMP.text = this.item.number.ToString();
        RT_NumberTMP.color = this.item.color;
        LB_NumberTMP.text = this.item.number.ToString();
        LB_NumberTMP.color = this.item.color;
        RB_NumberTMP.text = this.item.number.ToString();
        RB_NumberTMP.color = this.item.color;

        int order = transform.parent.GetComponent<Field>().child - 1; // �ڽ� ������Ʈ�� �߰��� ���̹Ƿ�
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
