using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class Filed : MonoBehaviour
{
    public static Filed Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] GameObject myField;
    [SerializeField] GameObject otherField;

    bool onMyFieldArea;

    private void Update()
    {
        DetectFieldArea();
    }

    //void SetOriginOrder(bool isMine)
    //{
    //    int count = isMine ? myCards.Count : otherCards.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        var targetCard = isMine ? myCards[i] : otherCards[i];
    //        targetCard?.GetComponent<Order>().SetOriginOrder(i);
    //    }
    //}

    void DetectFieldArea()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
        int layer = LayerMask.NameToLayer("MyFieldArea");
        onMyFieldArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }
}