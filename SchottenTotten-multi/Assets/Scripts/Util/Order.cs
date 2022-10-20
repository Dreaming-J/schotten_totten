using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Order : MonoBehaviour
{
    [SerializeField] Renderer[] backRenderers;
    [SerializeField] Renderer[] middleRenderers;
    [SerializeField] public string sortingLayerName;
    [SerializeField] PhotonView PV;
    int originOrder;

    public void SetOriginOrder(int originOrder, bool isSync = false)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
        if (isSync)
            PV?.RPC(nameof(SetOriginOrderRPC), RpcTarget.Others, originOrder);
    }

    [PunRPC] void SetOriginOrderRPC(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront)
    {
        SetOrder(isMostFront ? 100 : originOrder);
    }

    public void SetOrder(int order)
    {
        int mulOrder = order * 10;

        foreach (var renderer in backRenderers)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder;
        }

        foreach (var renderer in middleRenderers)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = mulOrder + 1;
        }
    }
}
