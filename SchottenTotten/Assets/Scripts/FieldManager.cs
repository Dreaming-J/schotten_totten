using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FieldManager : MonoBehaviour
{
    public static FieldManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] TileSO tileSO;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform tileSpawnPoint;
    [SerializeField] Transform tileLeft;
    [SerializeField] Transform tileRight;
    [SerializeField] List<GameObject> tiles;
    [SerializeField] GameObject fileds;
    [SerializeField] GameObject fieldcardPrefab;

    int[] order = new int[9] { 0, 8, 1, 7, 2, 6, 3, 5, 4 };
    float[] hard_offset = new float[2] { -6.7f, -7.45f };
    float[] hard_size = new float[2] { 7f, 5.5f };
    WaitForSeconds delay01 = new WaitForSeconds(0.1f);
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
        yield return delay1;
        TurnManager.Inst.EndTurn();
    }

    public IEnumerator SetTile()
    {
        CreateTile();
        int objCount = tiles.Count;
        float[] objLerps = new float[objCount];
        float interval = 1f / (objCount - 1);
        var targetRot = Utils.QI;

        for (int j = 0; j < objCount; j++)
        {
            int i = order[j];
            objLerps[i] = interval * i;
            var targetPos = Vector3.Lerp(tileLeft.position, tileRight.position, objLerps[i]);
            yield return delay01;
            MoveTransform(tiles[i], targetPos, targetRot, true, 0.7f);
        }
    }
    void CreateTile()
    {
        for (int i = 0; i < tileSO.tiles.Length; i++)
        {
            var tileObject = Instantiate(tilePrefab, tileSpawnPoint.position, Utils.QI);
            tileObject.name = i.ToString();
            tileObject.GetComponent<SpriteRenderer>().sprite = tileSO.tiles[i].sprite;
            tileObject.transform.parent = fileds.transform;
            tiles.Add(tileObject);
        }
    }

    public void MoveTransform(GameObject obj, Vector3 pos, Quaternion rot, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            obj.transform.DOMove(pos, dotweenTime);
            obj.transform.DORotateQuaternion(rot, dotweenTime);
        }
        else
        {
            obj.transform.position = pos;
            obj.transform.rotation = rot;
        }
    }

    public bool SpawnCard(bool isMine, Card card) //적 카드 내는거 수정중
    {
        if (!isMine)
        {
            Field field = tiles[1].transform.GetChild(1).GetComponent<Field>();
            var fieldcardObject = Instantiate(fieldcardPrefab, field.SpawnPos(), Utils.QI);
            var fieldcard = fieldcardObject.GetComponent<FieldCard>();
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
                field.fieldcards.Add(fieldcard);
                field.CheckStone();
                UpdateCollider(field);
                //StartCoroutine(CheckGameStatus());
                return true;
            }
        }

        return false;
    }

    void UpdateCollider(Field field)
    {
        if (!field.isFieldActive)
        {
            field.GetComponent<BoxCollider2D>().enabled = false;
            return;
        }
        Vector2 collideroffset = field.GetComponent<BoxCollider2D>().offset;
        Vector2 collidersize = field.GetComponent<BoxCollider2D>().size;
        collideroffset.y = hard_offset[field.child - 1];
        collidersize.y = hard_size[field.child - 1];
        field.GetComponent<BoxCollider2D>().offset = collideroffset;
        field.GetComponent<BoxCollider2D>().size = collidersize;
    }

    IEnumerator CheckGameStatus() //수정 중
    {
        yield return delay1;
        foreach (var tiles in tiles)
        {
            Field myfield = tiles.transform.GetChild(0).GetComponent<Field>();
            Field otherfield = tiles.transform.GetChild(1).GetComponent<Field>();
            if (myfield.isFieldActive || otherfield.isFieldActive)
                yield break;
        }
        
        //if (myBossEntity.isDie)
        //    StartCoroutine(GameManager.Inst.GameOver(false));

        //if (otherBossEntity.isDie)
        //    StartCoroutine(GameManager.Inst.GameOver(true));
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