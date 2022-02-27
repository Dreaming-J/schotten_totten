using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// ġƮ, UI, ��ŷ, ���ӿ���
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    private void Awake() => Inst = this;

    [Multiline(10)]
    [SerializeField] string cheatInfo;
    [SerializeField] NotificationPanel notificationPanel;
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] CameraEffect cameraEffect;
    [SerializeField] GameObject endTurnBtn; // ���� ����

    void Start()
    {
        //Screen.SetResolution(960, 540, false);
        Screen.SetResolution((Screen.width * 16) / 9, Screen.width, true);
        UISetup();
    }

    void UISetup()
    {
        notificationPanel.ScaleZero();
        resultPanel.ScaleZero();
        cameraEffect.SetGrayScale(false);
    }

    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    void InputCheatKey() // ���� �ȵǴ� ���
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            TurnManager.OnAddCard?.Invoke(true);

        if (Input.GetKeyDown(KeyCode.Keypad2))
            TurnManager.OnAddCard?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (CardManager.Inst.myPutCount == 0)
                CardManager.Inst.myPutCount = 1;
            TurnManager.Inst.EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
            CardManager.Inst.TryPutCard(false);

        if (Input.GetKeyDown(KeyCode.Keypad5))
            CardManager.Inst.myPutCount = 0;
    }

    public void StartGame()
    {
        StartCoroutine(TurnManager.Inst.StartGameCo());
        StartCoroutine(TileManager.Inst.SetTile());
    }

    public void Notification(string message)
    {
        notificationPanel.Show(message);
    }

    public IEnumerator GameOver(bool isMyWin)
    {
        TurnManager.Inst.isLoading = true;
        endTurnBtn.SetActive(false);
        yield return Utils.delay(2f);

        TurnManager.Inst.isLoading = true;
        resultPanel.Show(isMyWin ? "�¸�" : "�й�");
        cameraEffect.SetGrayScale(true);
    }
}
