using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] TMP_Text resultTMP;

    public void Show(string message)
    {
        resultTMP.text = message;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad);
    }

    public void Btn_Restart()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    void Start() => ScaleZero();

    [ContextMenu("ScaleOne")]
    public void ScaleOne() => transform.localScale = Vector3.one;

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;
}
