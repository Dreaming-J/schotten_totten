using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Field : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] GameObject field;
    [SerializeField] List<FieldCard> fieldcards;
    [SerializeField] public EFieldState eFieldState;

    public enum EFieldState { StraightFlush = 0, Triple = 1, Flush = 2, Straight = 3, ThreeCards = 4, Nothing = 5 };
    public int maxNum;

    public int child => field.transform.childCount;
    public bool isFieldActive => child < 3;
    public bool isMine => field.transform.name == "MyField";

    float[] hard_offset = new float[2] { -6.7f, -7.45f };
    float[] hard_size = new float[2] { 7f, 5.5f };
    float[] hard_pos = new float[3] { -5f, -6.5f, -8f };

    WaitForSeconds delay01 = new WaitForSeconds(0.1f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    private void Start()
    {
        eFieldState = EFieldState.Nothing;
    }

    public Vector3 SpawnPos()
    {
        Vector3 spawnPos = field.transform.position;
        spawnPos.y = isMine ? hard_pos[child] : -1 * hard_pos[child];
        UpdateCollider();
        return spawnPos;
    }

    void UpdateCollider()
    {
        if (child == 2) // 실행되는 타이밍이 자식 오브젝트로 추가하기 전
        {
            GetComponent<BoxCollider2D>().enabled = false;
            return;
        }
        Vector2 collideroffset = GetComponent<BoxCollider2D>().offset;
        Vector2 collidersize = GetComponent<BoxCollider2D>().size;
        collideroffset.y = isMine ? hard_offset[child] : -1 * hard_offset[child];
        collidersize.y = isMine ? hard_size[child] : -1 * hard_size[child];
        GetComponent<BoxCollider2D>().offset = collideroffset;
        GetComponent<BoxCollider2D>().size = collidersize;
    }

    public void CheckField(FieldCard fieldcard)
    {
        fieldcards.Add(fieldcard);
        if (isFieldActive)
            return;

        fieldcards.Sort((fieldcard1, fieldcard2) => fieldcard1.item.number.CompareTo(fieldcard2.item.number));
        maxNum = fieldcards[2].item.number;
        StartCoroutine(Reordering());
        SetCombination();
        tile.CheckTile();
    }

    public IEnumerator Reordering()
    {
        //yield return delay01; //AI가 내는 도중이랑 겹쳐서 오류생김; ; 그래서 일단 느리게 설정
        yield return delay07;
        for (int i = 0; i < child; i++)
        {
            var targetCard = fieldcards[i];
            targetCard?.GetComponent<Order>().SetOriginOrder(i);
            Vector3 pos = targetCard.transform.position;
            Quaternion rot = targetCard.transform.rotation;
            pos.y = isMine ? hard_pos[i] : -1 * hard_pos[i];
            targetCard?.MoveTransform(new PRS(pos, rot, Vector3.one), true, 0.7f);
        }
    }

    public void SetCombination()
    {
        bool isTriple = false;
        bool isFlush = false;
        bool isStraight = false;
        string[] colors = new string[3];
        int[] numbers = new int[3];
        for (int i = 0; i < fieldcards.Count; i++)
        {
            colors[i] = fieldcards[i].item.colorname;
            numbers[i] = fieldcards[i].item.number;
        }

        if (colors[0] != colors[1] && colors[1] != colors[2] && colors[2] != colors[0])
        {
            if (numbers[0] == numbers[1] && numbers[1] == numbers[2] && numbers[2] == numbers[0])
                isTriple = true;
        }
        if (colors[0] == colors[1] && colors[1] == colors[2])
            isFlush = true;
        if (numbers[0] + 1 == numbers[1] && numbers[2] - 1 == numbers[1])
            isStraight = true;
        SetEFieldState(isTriple, isFlush, isStraight);
    }

    void SetEFieldState(bool isTriple,bool isFlush,bool isStraight)
    {
        if (isTriple)
            eFieldState = EFieldState.Triple;

        else if (isFlush && isStraight)
            eFieldState = EFieldState.StraightFlush;

        else if (isFlush)
            eFieldState = EFieldState.Flush;

        else if (isStraight)
            eFieldState = EFieldState.Straight;

        else
            eFieldState = EFieldState.ThreeCards;
    }
}