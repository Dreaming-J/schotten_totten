using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// 치트, UI, 랭킹, 게임오버
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
        StartGame(); // NetworkManager_TMP로 사용시 주석처리
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !TileManager.Inst.isGameOver)
        {
            PhotonNetwork.Disconnect();
            LoadingSceneManager.LoadScene("LobbyScene", "상대방의 종료로 인해 로비화면으로 돌아갑니다.");
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
        resultPanel.Show(isMyWin ? "승리" : "패배");
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
