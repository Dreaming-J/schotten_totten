using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] GameObject tile;
    [SerializeField] GameObject myField;
    [SerializeField] GameObject otherField;
    [SerializeField] public ETileState eTileState;
    [SerializeField] EWhosFirst eWhosFirst;

    public enum ETileState { Mine, Yours, Nothing };
    public enum EWhosFirst { Mine, Yours, Nothing };

    private void Start()
    {
        eTileState = ETileState.Nothing;
        eWhosFirst = EWhosFirst.Nothing;
    }

    public void CheckTile(string fieldname)
    {
        Field myfield = myField.GetComponent<Field>();
        Field otherfield = otherField.GetComponent<Field>();
        if (eWhosFirst == EWhosFirst.Nothing)
            eWhosFirst = fieldname == "Field 0" ? EWhosFirst.Mine : EWhosFirst.Yours;

        if (myfield.eFieldState == Field.EFieldState.Nothing || otherfield.eFieldState == Field.EFieldState.Nothing)
            return;

        if ((int)myfield.eFieldState < (int)otherfield.eFieldState)
            eTileState = ETileState.Mine;

        else if ((int)myfield.eFieldState > (int)otherfield.eFieldState)
            eTileState = ETileState.Yours;

        else if ((int)myfield.eFieldState == (int)otherfield.eFieldState)
        {
            if (myfield.maxNum > otherfield.maxNum)
                eTileState = ETileState.Mine;
            else if (myfield.maxNum < otherfield.maxNum)
                eTileState = ETileState.Yours;
            else if (myfield.maxNum == otherfield.maxNum)
            {
                if (eWhosFirst == EWhosFirst.Mine)
                    eTileState = ETileState.Mine;
                else if (eWhosFirst == EWhosFirst.Yours)
                    eTileState = ETileState.Yours;
            }
        }
        StartCoroutine(SetTileWinner());
        StartCoroutine(TileManager.Inst.CheckGameStatus());
    }

    public IEnumerator SetTileWinner()
    {
        yield return Utils.delay(0.5f);
        if (eTileState == ETileState.Mine)
        {
            tile.GetComponent<SpriteRenderer>().color = new Color32(56, 56, 56, 255);
            myField.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (eTileState == ETileState.Yours)
        {
            tile.GetComponent<SpriteRenderer>().color = new Color32(56, 56, 56, 255);
            otherField.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}