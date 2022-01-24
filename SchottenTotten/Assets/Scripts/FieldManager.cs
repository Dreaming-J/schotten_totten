using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class FieldManager : MonoBehaviour
{
    public static FieldManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] GameObject fieldcardPrefab;

    WaitForSeconds delay05 = new WaitForSeconds(0.5f);
    WaitForSeconds delay1 = new WaitForSeconds(1);

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        if (!myTurn)
            StartCoroutine(AICo());
    }

    IEnumerator AICo()
    {
        yield return delay05;
        CardManager.Inst.TryPutCard(false);
        yield return delay1;
        TurnManager.Inst.EndTurn();
    }

    public bool SpawnCard(bool isMine, Card card)
    {
        List<GameObject> tiles = TileManager.Inst.tiles;
        if (!isMine) //AI 카드 내기
        {
            int check = 0;
            foreach (var a in tiles)
            {
                if (a.transform.GetChild(1).GetComponent<Field>().isFieldActive == false)
                    check += 1;
            }
            if (check == tiles.Count)
                return false;

            Field field;
            while (true)
            {
                field = tiles[Random.Range(0, tiles.Count)].transform.GetChild(1).GetComponent<Field>();
                if (field != null && !field.isMine && field.isFieldActive)
                    break;
            }
            card.originPRS.rot = card.originPRS.rot * Utils.Q180;
            var fieldcardObject = Instantiate(fieldcardPrefab, card.originPRS.pos, card.originPRS.rot);
            var fieldcard = fieldcardObject.GetComponent<FieldCard>();
            fieldcard.MoveTransform(new PRS(field.SpawnPos(), Utils.Q180, Vector3.one), true, 0.7f);
            fieldcard.transform.parent = field.transform;
            fieldcard.Setup(card.item);
            field.CheckField(fieldcard);
            return true;
        }

        foreach (var hit in Physics2D.RaycastAll(Utils.MousePos, Vector3.forward))
        {
            Field field = hit.collider?.GetComponent<Field>();
            FieldCard fieldcards = hit.collider?.GetComponent<FieldCard>();
            if (fieldcards != null)
                field = fieldcards.transform.parent.gameObject.GetComponent<Field>();
            
            if (field != null && field.isMine && field.isFieldActive)
            {
                var fieldcardObject = Instantiate(fieldcardPrefab, field.SpawnPos(), Utils.QI);
                var fieldcard = fieldcardObject.GetComponent<FieldCard>();
                fieldcard.transform.parent = field.transform;
                fieldcard.Setup(card.item);
                field.CheckField(fieldcard);
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