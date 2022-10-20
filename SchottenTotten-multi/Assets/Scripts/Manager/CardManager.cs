using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

public class CardManager : MonoBehaviourPunCallbacks
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    public ItemSO itemSO;
    public GameObject myHand;
    public GameObject otherHand;
    public List<Card> myCards;
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] ECardState eCardState;
    public ESortState esortState;
    [SerializeField] PhotonView PV;


    public List<Item> itemBuffer = new List<Item>(54);
    Card selectCard;
    Field targetField;
    bool isMyCardDrag;
    bool onMyCardArea;
    enum ECardState { Nothing, CanMouseOver, CanMouseDrag }
    public enum ESortState { ColorNum, NumColor }
    public int myPutCount;

    public Item PopItem()
    {
        Item item = itemBuffer[0];
        PV.RPC(nameof(SyncPop), RpcTarget.AllBuffered);
        return item;
    }
    [PunRPC] void SyncPop()
    {
        itemBuffer.RemoveAt(0);
    }

    public void SetupItemBuffer()
    {
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            Item item = itemSO.items[i];
            itemBuffer.Add(item);
        }
        for (int i = 0; i < itemBuffer.Count; i++)
        {
            int rand = Random.Range(i, itemBuffer.Count);
            Item temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }
        string jdata = JsonUtility.ToJson(new Serialization<Item>(itemBuffer));
        PV.RPC(nameof(UpdateItemBuffer), RpcTarget.OthersBuffered, jdata);
    }
    [PunRPC] void UpdateItemBuffer(string jdata)
    {
        itemBuffer = JsonUtility.FromJson<Serialization<Item>>(jdata).target;
        for (int i = 0; i < itemBuffer.Count; i++)
        {
            int num = (itemBuffer[i].number - 1) * 6;
            for (int j = 0; j < 6; j++)
            {
                if (itemBuffer[i].colornum == itemSO.items[num + j].colornum)
                {
                    itemBuffer[i].sprite = itemSO.items[num + j].sprite;
                    break;
                }
            }
        }
    }

    void Start()
    {
        TurnManager.OnAddCard += AddCard;
        TurnManager.OnTurnStarted += OnTurnStarted;
    }

    private void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCard;
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        if (myTurn)
            myPutCount = 0;
    }

    private void Update()
    {
        if (isMyCardDrag)
            CardDrag();

        DetectCardArea();
        SetECardState();
    }

    void AddCard(bool isMine)
    {
        if (itemBuffer.Count == 0)
            return;
        var cardObject = PhotonNetwork.Instantiate("Card", cardSpawnPoint.position, Utils.QI, 0);
        var card = cardObject.GetComponent<Card>();
        card.transform.parent = myHand.transform;
        myCards.Add(card);
        card.Setup(PopItem());
        myCards.Sort(Utils.CompareCard); //Card.cs의 Setup함수에서 RPC 통신 지연에 따른 보상 처리

        SetOriginOrder(isMine);
        CardAlignment(isMine);
    }

    void SetOriginOrder(bool isMine)
    {
        int count = myCards.Count;
        for (int i = 0; i < count; i++)
        {
            var targetCard = myCards[i];
            targetCard?.GetComponent<Order>().SetOriginOrder(i, true);
        }
    }

    void CardAlignment(bool isMine)
    {
        List<PRS> originCardPRSs = new List<PRS>();
        originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one);

        var targetCards = myCards;
        for (int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Utils.QI;
            if (objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }

        return results;
    }

    public bool TryPutCard(bool isMine)
    {
        if (isMine && myPutCount >= 1)
            return false;
        Card card = selectCard;
        var targetCards = myCards;


        if (FieldManager.Inst.SpawnCard(isMine, card))
        {
            targetCards.Remove(card);
            card.transform.DOKill();
            PhotonNetwork.Destroy(card.gameObject);
            if (isMine)
            {
                selectCard = null;
                myPutCount++;
            }
            CardAlignment(isMine);
            return true;
        }
        else
        {
            targetCards.ForEach(x => x.GetComponent<Order>().SetMostFrontOrder(false));
            CardAlignment(isMine);
            return false;
        }
    }



    #region 카드 움직임

    public void CardMouseOver(Card card)
    {
        if (eCardState == ECardState.Nothing)
            return;

        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)
    {
        EnlargeCard(false, card);
    }

    public void CardMouseDown()
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;
        isMyCardDrag = true;
    }

    public void CardMouseUp()
    {
        isMyCardDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (!onMyCardArea)
            if (TryPutCard(true))
                TurnManager.Inst.EndTurn();
    }

    void CardDrag()
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (!onMyCardArea)
        {
            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false);
        }
    }

    void DetectCardArea()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
        int layer = LayerMask.NameToLayer("MyCardArea");
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }

    void EnlargeCard(bool isEnlarge, Card card)
    {
        if (isEnlarge)
        {
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, -8f, -10f);
            card.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 1.5f), false);
        }
        else
            card.MoveTransform(card.originPRS, false);

        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    void SetECardState()
    {
        if (TurnManager.Inst.isLoading)
            eCardState = ECardState.Nothing;

        else if (!TurnManager.Inst.myTurn || myPutCount == 1)
            eCardState = ECardState.CanMouseOver;

        else if (TurnManager.Inst.myTurn && myPutCount == 0)
            eCardState = ECardState.CanMouseDrag;
    }


    #endregion
}