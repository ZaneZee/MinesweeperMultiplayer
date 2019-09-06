using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    #region Variable Declarations

    [Space(15)]
    [Header("Board Settings")]

    [Range(1, 40)] public int newBoardWidth = 16;
    [Range(1, 40)] public int newBoardHeight = 16;

    [HideInInspector] public int boardWidth;
    [HideInInspector] public int boardHeight;

    [Range(0, 250)] public int bombNum = 40;
    [HideInInspector] public int flagNum = 0;

    // TODO: Guarantee a safe area with a certain amount of tiles without bombs

    [Space(5)]

    public bool generateNewBoard = false;

    [Space(5)]

    public bool revealBoard = false;
    bool revealVal;

    public bool safeStart;

    GameObject[,] cells;
    public int[,] cellData;
    public int[,] cellSymbol;

    [Space(15)]
    [Header("Tile Settings")]

    public GameObject tilePrefab;

    [Space(10)]

    [Tooltip("The default tile color")]
    public Color defaultColor;
    [Tooltip("The color when a tile holds a bomb")]
    public Color bombColor;
    [Tooltip("The tile color by number of neighboring tiles with a bomb")]
    public Color[] tileColor;

    #endregion

    #region Board Functions

    // Create Board
    void CreateBoard() {

        // Create a 2D array that will hold our tile Game Object
        cells = new GameObject[newBoardWidth, newBoardHeight];
        boardWidth = newBoardWidth;
        boardHeight = newBoardHeight;

        // Create a tile for how tall and wide our board is
        for (int i = 0; i < newBoardHeight; i++) {
            for (int j = 0; j < newBoardWidth; j++) {
                CreateTile(j, i);
            }
        }
    }

    // Clear Board
    void ClearBoard() {
        for (int i = 0; i < boardHeight; i++) {
            for (int j = 0; j < boardWidth; j++) {
                DeleteTile(j, i);
            }
        }
        flagNum = 0;
    }

    // Generate Bombs
    void GenerateBombs() {

        // Create the 2D array that will hold our bomb data
        cellData = new int[boardWidth, boardHeight];

        if (bombNum > boardWidth * boardHeight) {
            bombNum = boardWidth * boardHeight;
            Debug.Log("Number of bombs larger than number of tiles. Bomb number has been set to number of tiles.");
        }

        // Attempt to place a bomb for the amount of bombs we want on our board
        for (int i = 0; i < bombNum; i++) {

            bool bombPlaced = false;
            while (!bombPlaced) {

                int bombX = Random.Range(0, boardWidth);
                int bombY = Random.Range(0, boardHeight);

                if (cellData[bombX, bombY] == 0) {
                    PlaceBomb(bombX, bombY);
                    bombPlaced = true;
                }
            }
        }
    }

    // Clear Bombs
    void ClearBombs() {
        for (int i = 0; i < boardHeight; i++) {
            for (int j = 0; j < boardWidth; j++) {
                ClearBomb(j, i);
            }
        }
    }

    // Conceal Tiles
    void ConcealTiles() {
        for (int i = 0; i < boardHeight; i++) {
            for (int j = 0; j < boardWidth; j++) {
                ConcealTile(j, i);
            }
        }
    }

    // Reveal Tiles
    void RevealTiles() {
        for (int i = 0; i < boardHeight; i++) {
            for (int j = 0; j < boardWidth; j++) {
                RevealTile(j, i);
            }
        }
    }

    // Generate Numbers
    void GenerateNumbers() {

        // Create the 2D array that will hold our number data
        cellSymbol = new int[boardWidth, boardHeight];

        for (int i = 0; i < boardHeight; i++) {
            for (int j = 0; j < boardWidth; j++) {
                GenerateNumber(j, i);
                SetSymbol(j, i, cells[j, i].GetComponent<Tile>().neighborBombs);
            }
        }
    }

    // Readjust Camera
    void ReadjustCamera() {
        Camera c = Camera.main;
        int s = (boardWidth > boardHeight) ? boardWidth : boardHeight;
        c.transform.position = new Vector3(boardWidth / 2, -boardHeight / 2, -s);
    }

    // Choose Safe Start
    void SafeStart() {

        bool canPlace = false;
        for (int i = 0; i < boardHeight; i++) {
            for (int j = 0; j < boardWidth; j++) {
                if (cells[i, j].GetComponent<Tile>().neighborBombs == 0 &&
                    cells[i, j].GetComponent<Tile>().hasBomb == 0 &&
                    !cells[i, j].GetComponent<Tile>().revealed) {
                    canPlace = true;
                }
            }
        }
        if (!canPlace) {
            Debug.Log("Cannot Perform Safe Start");
        }
        else {
            bool startPlaced = false;
            while (!startPlaced) {
                int startX = Random.Range(0, boardWidth);
                int startY = Random.Range(0, boardHeight);

                if (cells[startX, startY].GetComponent<Tile>().neighborBombs == 0 &&
                    cells[startX, startY].GetComponent<Tile>().hasBomb == 0 &&
                    !cells[startX, startY].GetComponent<Tile>().revealed) {
                    ClearField(startX, startY);
                    startPlaced = true;
                }
            }
        }
    }

    #endregion

    #region Tile Functions

    // Create Tile
    void CreateTile(int x, int y) {

        // Create GameObject
        cells[x, y] = Instantiate(tilePrefab, gameObject.transform);
        cells[x, y].name = x.ToString() + " , " + y.ToString();


        // Set position
        Vector3 postion = new Vector3(x, -y, 0);
        cells[x, y].transform.position = postion;

        // Set Values
        cells[x, y].GetComponent<Tile>().x = x;
        cells[x, y].GetComponent<Tile>().y = y;
        cells[x, y].GetComponent<Tile>().hasBomb = 0;
        cells[x, y].GetComponent<Tile>().ConcealVal();
    }

    // Delete Tile
    void DeleteTile(int x, int y) {
        ClearBomb(x, y);
        Destroy(cells[x, y]);
    }

    // Place Bomb
    void PlaceBomb(int x, int y) {
        cellData[x, y] = 1;
        cells[x, y].GetComponent<Tile>().SetData(1);
        flagNum++;
        Debug.Log("Place Bomb Called!");
    }

    // Clear Bomb
    void ClearBomb(int x, int y) {
        cellData[x, y] = 0;
        cells[x, y].GetComponent<Tile>().SetData(0);
        flagNum--;
    }

    // Conceal Tile
    void ConcealTile(int x, int y) {
        cells[x, y].GetComponent<Tile>().ConcealVal();
    }

    // Reveal Tile
    void RevealTile(int x, int y) {
        cells[x, y].GetComponent<Tile>().RevealVal();
    }

    // Generate Number
    void GenerateNumber(int x, int y) {
        int leftIndex = x - 1;
        int rightIndex = x + 1;
        int upIndex = y - 1;
        int downIndex = y + 1;
        int activeNeighbors = 0;

        // top row
        if (upIndex >= 0) {
            activeNeighbors += cellData[x, upIndex];
            activeNeighbors += (leftIndex >= 0) ? cellData[leftIndex, upIndex] : 0;
            activeNeighbors += (rightIndex < boardWidth) ? cellData[rightIndex, upIndex] : 0;
        }

        // middle row (not including self)
        activeNeighbors += (leftIndex >= 0) ? cellData[leftIndex, y] : 0;
        activeNeighbors += (rightIndex < boardWidth) ? cellData[rightIndex, y] : 0;

        // bottom row
        if (downIndex < boardHeight) {
            activeNeighbors += cellData[x, downIndex];
            activeNeighbors += (leftIndex >= 0) ? cellData[leftIndex, downIndex] : 0;
            activeNeighbors += (rightIndex < boardWidth) ? cellData[rightIndex, downIndex] : 0;
        }

        cellSymbol[x, y] = activeNeighbors;
        cells[x, y].GetComponent<Tile>().SetNeighbors(activeNeighbors);
    }

    // Set Symbol
    void SetSymbol(int x, int y, int activeNeighbors) {
        cells[x, y].GetComponent<Tile>().SetSymbol(activeNeighbors);
    }

    // TODO: Try to make this more clean
    // Find Empty Field Tiles From Point
    List<GameObject> FindEmptyFieldTilesFromPoint(int x, int y, List<GameObject> emptyField) {

        emptyField.Add(cells[x, y]);

        // if the current tile is 0
        if (cells[x, y].GetComponent<Tile>().neighborBombs == 0) {

            List<GameObject> neighborList = new List<GameObject>();

            // Add surrounding tiles to temporary list

            // Set Indexes
            int leftIndex = x - 1;
            int rightIndex = x + 1;
            int upIndex = y - 1;
            int downIndex = y + 1;

            // add top row
            if (upIndex >= 0) {

                // left
                if (leftIndex >= 0) {
                    neighborList.Add(cells[leftIndex, upIndex]);
                }

                // middle
                neighborList.Add(cells[x, upIndex]);

                // right
                if (rightIndex < boardWidth) {
                    neighborList.Add(cells[rightIndex, upIndex]);
                }
            }

            // add middle row
            // left
            if (leftIndex >= 0) {
                neighborList.Add(cells[leftIndex, y]);
            }

            // right
            if (rightIndex < boardWidth) {
                neighborList.Add(cells[rightIndex, y]);
            }

            // add bottom row
            if (downIndex < boardHeight) {

                // left
                if (leftIndex >= 0) {
                    neighborList.Add(cells[leftIndex, downIndex]);
                }

                // middle
                neighborList.Add(cells[x, downIndex]);

                // right
                if (rightIndex < boardWidth) {
                    neighborList.Add(cells[rightIndex, downIndex]);
                }
            }

            // For every neighbor tile
            foreach (GameObject tile in neighborList) {

                // Only use the tiles that aren't already in the list
                if (emptyField.IndexOf(tile) < 0) {

                    // Call this function using neighboring tiles as an input
                    FindEmptyFieldTilesFromPoint(tile.GetComponent<Tile>().x, tile.GetComponent<Tile>().y, emptyField);
                }
            }
        }

        return emptyField;

    }

    // Clear Field from Point
    public void ClearField(int x, int y) {
        List<GameObject> newList = new List<GameObject>();
        newList = FindEmptyFieldTilesFromPoint(x, y, newList);
        foreach (GameObject tile in newList) {
            tile.GetComponent<Tile>().RevealVal();
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start() {
        CreateBoard();
        GenerateBombs();
        GenerateNumbers();
        ReadjustCamera();
    }

    // Update is called once per frame
    void Update() {

        if (generateNewBoard) {
            ClearBoard();
            CreateBoard();
            GenerateBombs();
            GenerateNumbers();
            ReadjustCamera();
            if (revealBoard) { RevealTiles(); } else { ConcealTiles(); }
            generateNewBoard = false;
        }

        if (revealVal != revealBoard) {
            if (revealBoard) { RevealTiles(); } else { ConcealTiles(); }
        }
        revealVal = revealBoard;

        if (safeStart) {
            SafeStart();
            safeStart = false;
        }

        Debug.Log("Flag Num: " + flagNum);
    }

}
