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

        int order = transform.parent.GetComponent<Field>().child;
        GetComponent<Order>().SetOriginOrder(order);
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
