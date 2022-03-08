using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Photon.Pun;

public class ThrobberPanel : MonoBehaviour
{
    [SerializeField] TMP_Text ThrobberText;
    [SerializeField] AnimationCurve CustomCurve;

    public void Show(bool isOn)
    {
        if (isOn)
        {
            transform.DOScale(Vector3.one, 0.5f).SetEase(CustomCurve);
        }
        else
        {
            transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutQuad);
        }
    }

    [ContextMenu("ScaleOne")]
    public void ScaleOne() => transform.localScale = Vector3.one;

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;
}
