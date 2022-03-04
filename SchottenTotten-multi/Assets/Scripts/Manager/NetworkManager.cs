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
    [SerializeField] [Tooltip("��ü ȭ�� ���θ� ���մϴ�.")] bool isFull;
    [SerializeField] [Tooltip("���� ������ ���� ��� �ð��� ���մϴ�.")] int waitingtime;

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



    #region ���� ����
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
            LobbyInfoText.text = "�κ� " + PhotonNetwork.CountOfPlayersOnMaster + "�� / ������ " + PhotonNetwork.CountOfPlayers + "��";
        if (isEnteredRoom) RoomRenewal();
    }

    public void Btn_Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "�� ȯ���մϴ�";
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



    #region �渮��Ʈ ����
    // ����ư -2 , ����ư -1 , �� ����
    public void Btn_MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // �ִ�������
        maxPage = myList.Count % CellBtn.Length == 0 ? (myList.Count == 0 ? 0 : myList.Count / CellBtn.Length - 1) : myList.Count / CellBtn.Length;

        // ����, ������ư
        PrevBtn.interactable = (currentPage <= 0) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
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
            if (!roomList[i].RemovedFromList) // �����ϴ� ��(�����ؼ� �ȵǴ� ��)
            {
                if (!myList.Contains(roomList[i]))
                    myList.Add(roomList[i]);
                else
                    myList[myList.IndexOf(roomList[i])] = roomList[i]; // myList ������Ʈ
            }
            else if (myList.IndexOf(roomList[i]) != -1)
                myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion



    #region ��
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
        ReadyBtn.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "�غ�";
        RoomPanel.SetActive(true);
        isEnteredRoom = true;
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; Btn_CreateRoom(); }

    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; Btn_CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer) // �ٸ� �÷��̾� ���� ��
    {
        ChatRPC("<color=yellow>" + newPlayer.NickName + "���� �����ϼ̽��ϴ�</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // �ٸ� �÷��̾� ���� ��
    {
        isOtherReady = false;
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "���� �����ϼ̽��ϴ�</color>");
    }

    void RoomRenewal()
    {
        PlayerInfoText.text = "<color=yellow>" + PhotonNetwork.LocalPlayer.NickName + ": " + (isMeReady ? "�غ� �Ϸ�" : "�غ� ��") + "</color>\n\n";
        PlayerInfoText.text += PhotonNetwork.PlayerListOthers.Length == 1 ? (PhotonNetwork.PlayerListOthers[0].NickName + ": " + (isOtherReady ? "�غ� �Ϸ�" : "�غ� ��")) : "...";
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "�� / " + PhotonNetwork.CurrentRoom.MaxPlayers + "�ִ�";
        if (isMeReady && isOtherReady && !isReady2Start)
        {
            isReady2Start = true;
            StartCoroutine(Ready2Start(true));
        }
    }
    #endregion



    #region ä��
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



    #region �غ�
    public void Btn_Ready()
    {
        isMeReady = !isMeReady;
        ReadyBtn.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = isMeReady ? "���" : "�غ�";
        PV.RPC("ReadyRPC", RpcTarget.OthersBuffered, isMeReady);
    }

    [PunRPC] void ReadyRPC(bool ismeready)
    {
        isOtherReady = ismeready;
    }

    IEnumerator Ready2Start(bool isStart)
    {
        PV.RPC("ChatRPC", RpcTarget.Others, "������ ���۵˴ϴ�");
        for (int i = waitingtime; i > 0; i--)
        {
            PV.RPC("ChatRPC", RpcTarget.Others, i + "��");
            yield return new WaitForSeconds(1);
            if (!(isMeReady && isOtherReady))
            {
                isReady2Start = false;
                PV.RPC("ChatRPC", RpcTarget.Others, "<color=red>������ ��ҵǾ����ϴ�.</color>");
                yield break;
            }
        }
        LoadingSceneManager.LoadScene("GameScene", "������ ���۵˴ϴ�.");
    }
    #endregion



    #region Ű����
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