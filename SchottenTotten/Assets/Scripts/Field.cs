using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] GameObject field;
    [SerializeField] public List<FieldCard> fieldcards;

    public int child => field.transform.childCount;
    public bool isFieldActive => child < 3;
    public bool isMine => field.transform.name == "MyField";

    public Vector3 SpawnPos()
    {
        Vector3 spawnPos = field.transform.position;
        if (child == 0)
            spawnPos.y = isMine ? -5f : 5f;
        else if (child == 1)
            spawnPos.y = isMine ? -6.5f : 6.5f;
        else if (child == 2)
            spawnPos.y = isMine ? -8f : 8f;
        return spawnPos;
    }

    public void CheckStone()
    {
        //ī�� 6���� ���� ��� ī���� ���� �����ϴ� �ڵ�
    }
}