using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// ġƮ, UI, ��ŷ, ���ӿ���
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    private void Awake()
    {
        Inst = this;
        PreparePool();
    }

    [SerializeField] List<GameObject> Prefabs;
    [SerializeField] NotificationPanel notificationPanel;
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] CameraEffect cameraEffect;

    void Start()
    {
        UISetup();
        StartGame(); // NetworkManager_TMP�� ���� �ּ�ó��
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !TileManager.Inst.isGameOver)
        {
            PhotonNetwork.Disconnect();
            LoadingSceneManager.LoadScene("LobbyScene", "������ ����� ���� �κ�ȭ������ ���ư��ϴ�.");
        }
    }

    void UISetup()
    {
        notificationPanel.ScaleZero();
        resultPanel.ScaleZero();
        cameraEffect.SetGrayScale(false);
    }

    public void StartGame()
    {
        PhotonNetwork.Instantiate("User", new Vector3(0, -12.24678f, 0), Utils.QI, 0);
        StartCoroutine(TileManager.Inst.SetTile());
        StartCoroutine(TurnManager.Inst.StartGameCo());
    }

    public void Notification(string message)
    {
        notificationPanel.Show(message);
    }

    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        yield return Utils.delay(2f);

        TurnManager.Inst.isLoading = true;
        resultPanel.Show(isMyWin ? "�¸�" : "�й�");
        cameraEffect.SetGrayScale(true);
    }

    void PreparePool()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && this.Prefabs != null)
        {
            foreach (GameObject prefab in this.Prefabs)
            {
                pool.ResourceCache.Add(prefab.name, prefab);
            }
        }
    }
}
