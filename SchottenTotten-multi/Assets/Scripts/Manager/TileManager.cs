using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class TileManager : MonoBehaviour
{
    public static TileManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] List<Sprite> tilesprites;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform tileSpawnPoint;
    [SerializeField] Transform tileLeft;
    [SerializeField] Transform tileRight;
    [SerializeField] GameObject tile;

    public List<GameObject> tiles;
    public bool isGameOver = false;

    void CreateTile()
    {
        for (int i = 0; i < tilesprites.Count; i++)
        {
            var tileObject = Instantiate(tilePrefab, tileSpawnPoint.position, Utils.QI);
            tileObject.name = i.ToString();
            tileObject.GetComponent<SpriteRenderer>().sprite = tilesprites[i];
            tileObject.transform.parent = tile.transform;
            tiles.Add(tileObject);
        }
    }

    public IEnumerator SetTile()
    {
        CreateTile();
        int objCount = tiles.Count;
        float[] objLerps = new float[objCount];
        float interval = 1f / (objCount - 1);
        var targetRot = Utils.QI;

        for (int i = 0; i < objCount; i++)
        {
            objLerps[i] = interval * i;
            var targetPos = Vector3.Lerp(tileLeft.position, tileRight.position, objLerps[i]);
            yield return Utils.delay(0.1f);
            MoveTransform(tiles[i], new PRS(targetPos, targetRot, Vector3.one), true, 0.7f);
        }
    }

    public void MoveTransform(GameObject obj, PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            obj.transform.DOMove(prs.pos, dotweenTime);
            obj.transform.DORotateQuaternion(prs.rot, dotweenTime);
            obj.transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            obj.transform.position = prs.pos;
            obj.transform.rotation = prs.rot;
            obj.transform.localScale = prs.scale;
        }
    }

    public IEnumerator CheckGameStatus()
    {
        int mine = 0;
        int yours = 0;
        bool isMyWin = false;

        for (int i = 0; i < tiles.Count; i++)
        {
            Tile tile = tiles[i].GetComponent<Tile>();
            if (i != 0 && i != 8)
            {
                Tile prevtile = tiles[i-1].GetComponent<Tile>();
                Tile nexttile = tiles[i+1].GetComponent<Tile>();
                if (tile.eTileState != Tile.ETileState.Nothing && prevtile.eTileState == tile.eTileState && tile.eTileState == nexttile.eTileState)
                {
                    if (tile.eTileState == Tile.ETileState.Mine)
                        isMyWin = true;
                    if (tile.eTileState == Tile.ETileState.Yours)
                        isMyWin = false;
                    isGameOver = true;
                    break;
                }
            }
            if (tile.eTileState == Tile.ETileState.Mine)
                mine += 1;
            else if (tile.eTileState == Tile.ETileState.Yours)
                yours += 1;
        }
        if (mine + yours == tiles.Count)
        {
            if (mine > yours)
                isMyWin = true;
            if (mine < yours)
                isMyWin = false;
            isGameOver = true;
        }

        yield return Utils.delay(1f);

        if (isGameOver)
            StartCoroutine(GameManager.Inst.GameOver(isMyWin));
    }

    public static int TransTileName(int tilenum)
    {
        int cen = 4;
        int interval = (cen - tilenum) > 0 ? (cen - tilenum) : (cen - tilenum) * -1;
        if (tilenum < cen)
            cen += interval;
        else if (tilenum > cen)
            cen -= interval;
        return cen;
    }
}