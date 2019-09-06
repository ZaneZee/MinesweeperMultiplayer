using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    //kappa
    #region Variable Declarations

    private Board board;

    [Header("Coordinates")]
    public int x;
    public int y;

    [Header("Info")]
    public int hasBomb = 0;
    public int neighborBombs = 0;

    public bool revealed = false;
    public bool isFlagged = false;

    private Color symbolColor;
    private string symbolString;

    #endregion

    #region Public Tile Functions

    public void SetData(int d) {
        hasBomb = d;
    }

    public void SetNeighbors(int n) {
        neighborBombs = n;
    }

    public void SetSymbol(int s) {
        if (hasBomb == 0) {
            symbolString = s.ToString();
            symbolColor = board.tileColor[s];
        }
        else {
            symbolString = "B";
            symbolColor = board.bombColor;
        }
    }

    public void ConcealVal() {
        revealed = false;
        this.GetComponent<Renderer>().material.color = board.defaultColor;
        this.GetComponentInChildren<TextMesh>().text = "";
    }

    public void RevealVal() {
        if (isFlagged) {
            RemoveFlagTile();
        }
        revealed = true;
        this.GetComponent<Renderer>().material.color = symbolColor;
        this.GetComponentInChildren<TextMesh>().text = symbolString;
    }

    public void FlagTile() {
        board.flagNum--;
        isFlagged = true;
        GetComponentInChildren<TextMesh>().text = "F";
    }

    public void RemoveFlagTile() {
        board.flagNum++;
        isFlagged = false;
        ConcealVal();
    }
    #endregion

    #region Event Functions

    private void OnEnable() {
        board = this.transform.parent.GetComponent<Board>();
    }

    //When mouse is over the game object
    private void OnMouseOver() {

        //if you press the "Interact" key, do interact logic
        if (Input.GetButtonDown("Interact")) {
            if (!revealed) {
                if (!isFlagged) {
                    if (hasBomb == 0) {
                        if (neighborBombs == 0) {
                            board.ClearField(x, y);
                        }
                    }
                    else {
                        Debug.Log("Kaboom");
                    }
                    RevealVal();
                }
            }
        }

        //if you press the "Flag" key, do flag logic
        if (Input.GetButtonDown("Flag")) {
            if (!revealed) {
                if (isFlagged) {
                    RemoveFlagTile();
                }
                else {
                    if (board.flagNum > 0) {
                        FlagTile();
                    }
                }
            }
        }

    }

    #endregion
}
