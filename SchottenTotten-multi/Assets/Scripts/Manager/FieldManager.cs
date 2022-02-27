using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using Photon.Pun;

public class FieldManager : MonoBehaviour
{
    public static FieldManager Inst { get; private set; }
    void Awake() => Inst = this;

    public bool SpawnCard(bool isMine, Card card)
    {
        foreach (var hit in Physics2D.RaycastAll(Utils.MousePos, Vector3.forward))
        {
            Field field = hit.collider?.GetComponent<Field>();
            FieldCard fieldcards = hit.collider?.GetComponent<FieldCard>();
            if (fieldcards != null)
                field = fieldcards.transform.parent.gameObject.GetComponent<Field>();
            
            if (field != null && field.isMine && field.isFieldActive)
            {
                var fieldcardObject = PhotonNetwork.Instantiate("FieldCard", field.SpawnPos(), Utils.QI, 0); // 좀 더 깔끔하게 소환 못하나??
                var fieldcard = fieldcardObject.GetComponent<FieldCard>();
                int tilenum = int.Parse(field.transform.parent.name);
                fieldcard.Setup(card.item, tilenum);
                return true;
            }
        }
        return false;
    }

    public void FieldCardMouseOver(FieldCard fieldcard)
    {
        if (TurnManager.Inst.isLoading)
            return;

        EnlargeFieldCard(true, fieldcard);
    }

    public void FieldCardMouseExit(FieldCard fieldcard)
    {
        EnlargeFieldCard(false, fieldcard);
    }

    void EnlargeFieldCard(bool isEnlarge, FieldCard fieldcard)
    {
        fieldcard.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }
}