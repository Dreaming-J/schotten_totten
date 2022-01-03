using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] TileSO tileSO;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject background;
    [SerializeField] Transform tileSpawnPoint;
    [SerializeField] Transform tileLeft;
    [SerializeField] Transform tileRight;
    [SerializeField] List<GameObject> tiles;

    int[] order = new int[9] { 0, 8, 1, 7, 2, 6, 3, 5, 4 };
    WaitForSeconds delay01 = new WaitForSeconds(0.1f);

    void Start()
    {
        CreateTile();
        StartCoroutine(SetTile(tileLeft, tileRight, tiles.Count, Vector3.one));
    }

    void CreateTile()
    {
        for (int i = 0; i < tileSO.tiles.Length; i++)
        {
            var tileObject = Instantiate(tilePrefab, tileSpawnPoint.position, Utils.QI);
            tileObject.name = "tile_" + (i + 1).ToString();
            tileObject.GetComponent<SpriteRenderer>().sprite = tileSO.tiles[i].sprite;
            tileObject.transform.parent = background.transform;
            tiles.Add(tileObject);
        }
    }

    IEnumerator SetTile(Transform leftTr, Transform rightTr, int objCount, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        float interval = 1f / (objCount - 1);
        var targetRot = Utils.QI;

        for (int j = 0; j < objCount; j++)
        {
            int i = order[j];
            objLerps[i] = interval * i;
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            yield return delay01;
            MoveTransform(tiles[i], targetPos, targetRot, true, 0.7f);
        }
    }
    public void MoveTransform(GameObject obj, Vector3 pos, Quaternion rot, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            obj.transform.DOMove(pos, dotweenTime);
            obj.transform.DORotateQuaternion(rot, dotweenTime);
        }
        else
        {
            obj.transform.position = pos;
            obj.transform.rotation = rot;
        }
    }
}