using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Develop")]
    [SerializeField] [Tooltip("전체 화면 여부를 정합니다.")] bool isFull;
    [SerializeField] [Tooltip("게임 시작을 위한 대기 시간을 정합니다.")] int waitingtime;

    [Header("LoginPanel")]
    public GameObject LoginPanel;
    public InputField NickNameInput;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PrevBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text PlayerInfoText;
    public Button ReadyBtn;
    public Text RoomInfoText;
    public Text[] ChatText;
    public InputField ChatInput;

    [Header("ETC")]
    public PhotonView PV;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 0, maxPage, multiple;
    bool isMeReady = false, isOtherReady = false, isEnteredRoom = false, isReady2Start = false;



    #region 서버 연결
    private void Awake()
    {
        LoginPanel.SetActive(true);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        if (isFull)
            Screen.SetResolution((Screen.width * 16) / 9, Screen.width, true);
        else
            Screen.SetResolution(960, 540, false);
        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        InputKeyBoard();
        if (PhotonNetwork.InLobby)
            LobbyInfoText.text = "로비 " + PhotonNetwork.CountOfPlayersOnMaster + "명 / 접속자 " + PhotonNetwork.CountOfPlayers + "명";
        if (isEnteredRoom) RoomRenewal();
    }

    public void Btn_Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
    }

    public void Btn_Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        NickNameInput.text = "";
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }
    #endregion



    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void Btn_MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = myList.Count % CellBtn.Length == 0 ? (myList.Count == 0 ? 0 : myList.Count / CellBtn.Length - 1) : myList.Count / CellBtn.Length;

        // 이전, 다음버튼
        PrevBtn.interactable = (currentPage <= 0) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = currentPage * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList) // 존재하는 방(삭제해선 안되는 방)
            {
                if (!myList.Contains(roomList[i]))
                    myList.Add(roomList[i]);
                else
                    myList[myList.IndexOf(roomList[i])] = roomList[i]; // myList 업데이트
            }
            else if (myList.IndexOf(roomList[i]) != -1)
                myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion



    #region 방
    public void Btn_CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 10) : RoomInput.text, new RoomOptions { MaxPlayers = 2, PublishUserId = true });

    public void Btn_JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void Btn_LeaveRoom()
    {
        isMeReady = false;
        isOtherReady = false;
        isEnteredRoom = false;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        ReadyBtn.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "준비";
        RoomPanel.SetActive(true);
        isEnteredRoom = true;
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; Btn_CreateRoom(); }

    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; Btn_CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer) // 다른 플레이어 입장 시
    {
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 다른 플레이어 퇴장 시
    {
        isOtherReady = false;
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }

    void RoomRenewal()
    {
        PlayerInfoText.text = "<color=yellow>" + PhotonNetwork.LocalPlayer.NickName + ": " + (isMeReady ? "준비 완료" : "준비 중") + "</color>\n\n";
        PlayerInfoText.text += PhotonNetwork.PlayerListOthers.Length == 1 ? (PhotonNetwork.PlayerListOthers[0].NickName + ": " + (isOtherReady ? "준비 완료" : "준비 중")) : "...";
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
        if (isMeReady && isOtherReady && !isReady2Start)
        {
            isReady2Start = true;
            StartCoroutine(Ready2Start(true));
        }
    }
    #endregion



    #region 채팅
    public void Btn_SendMsg()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + ": " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput)
        {
            for (int i = 1; i < ChatText.Length; i++)
                ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion



    #region 준비
    public void Btn_Ready()
    {
        isMeReady = !isMeReady;
        ReadyBtn.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = isMeReady ? "취소" : "준비";
        PV.RPC("ReadyRPC", RpcTarget.OthersBuffered, isMeReady);
    }

    [PunRPC] void ReadyRPC(bool ismeready)
    {
        isOtherReady = ismeready;
    }

    IEnumerator Ready2Start(bool isStart)
    {
        PV.RPC("ChatRPC", RpcTarget.Others, "게임이 시작됩니다");
        for (int i = waitingtime; i > 0; i--)
        {
            PV.RPC("ChatRPC", RpcTarget.Others, i + "초");
            yield return new WaitForSeconds(1);
            if (!(isMeReady && isOtherReady))
            {
                isReady2Start = false;
                PV.RPC("ChatRPC", RpcTarget.Others, "<color=red>게임이 취소되었습니다.</color>");
                yield break;
            }
        }
        LoadingSceneManager.LoadScene("GameScene", "게임이 시작됩니다.");
    }
    #endregion



    #region 키맵핑
    void InputKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            if (!PhotonNetwork.IsConnected)
                Btn_Connect();
            else if (PhotonNetwork.InLobby)
                Btn_CreateRoom();
            else if (PhotonNetwork.InRoom)
                Btn_SendMsg();
        }

        if (PhotonNetwork.InRoom && Input.GetKeyDown(KeyCode.F5))
            Btn_Ready();
    }
    #endregion
}