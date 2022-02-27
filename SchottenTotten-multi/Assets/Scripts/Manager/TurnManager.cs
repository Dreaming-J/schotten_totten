using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Photon.Pun;
using Photon.Realtime;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager Inst { get; private set; }
    void Awake() => Inst = this;

    [Header("Develop")]
    [SerializeField] [Tooltip("시작 턴 모드를 정합니다.")] ETurnMode eTurnMode;
    [SerializeField] [Tooltip("시작 카드 개수를 정합니다.")] int startCardCount;

    [Header("Properties")]
    public bool isLoading;
    public bool myTurn;

    enum ETurnMode { Random, My, Other }
    PhotonView PV;

    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void GameSetup()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0, -12.24678f, 0), Utils.QI, 0);

        if (PhotonNetwork.IsMasterClient)
        {
            switch (eTurnMode)
            {
                case ETurnMode.Random:
                    myTurn = Random.Range(0, 2) == 0;
                    break;
                case ETurnMode.My:
                    myTurn = true;
                    break;
                case ETurnMode.Other:
                    myTurn = false;
                    break;
            }
            PV.RPC(nameof(SyncTurn), RpcTarget.Others, myTurn, false);
        }
        CardManager.Inst.SetupItemBuffer();
    }

    public IEnumerator StartGameCo()
    {
        GameSetup();
        isLoading = true;
        yield return Utils.delay(0.5f);
        for (int i = 0; i < startCardCount; i++)
        {
            for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
            {
                yield return Utils.delay(0.2f);
                if (Utils.ID == PhotonNetwork.PlayerList[j].UserId)
                    OnAddCard?.Invoke(true);
            }
        }
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo()
    {
        isLoading = true;
        if (myTurn)
        {
            yield return Utils.delay(0.7f);
            GameManager.Inst.Notification(PhotonNetwork.LocalPlayer.NickName + " 턴");
        }
        yield return Utils.delay(0.7f);
        isLoading = false;

        OnTurnStarted?.Invoke(myTurn);
    }

    public void EndTurn()
    {
        if (myTurn && CardManager.Inst.myPutCount != 1)
            return;
        OnAddCard?.Invoke(myTurn);
        PV.RPC(nameof(SyncTurn), RpcTarget.All, myTurn, true);
    }

    [PunRPC] public void SyncTurn(bool myturn, bool startcoroutine)
    {
        if (startcoroutine)
        {
            myTurn = !myTurn;
            StartCoroutine(StartTurnCo());
        }
        else
            myTurn = !myturn;

    }
}
