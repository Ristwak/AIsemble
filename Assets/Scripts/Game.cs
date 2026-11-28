using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class Game : MonoBehaviour
{
    [Header("Game UI")]
    public GameObject comingSoonPanel;
    public float delayBeforePanel = 2f;

    [Header("Puzzle Pieces")]
    public GameObject[] go;

    [Header("Movement")]
    public float AnimSpeed = 10f;

    private Piece[,] Matrix =
        new Piece[Constants.MaxRows, Constants.MaxColumns];

    private readonly Dictionary<GameObject, Piece> pieceLookup =
        new Dictionary<GameObject, Piece>();

    private GameState gameState;

    private Camera mainCamera;

    private int blankRow;
    private int blankColumn;

    private int targetRow;
    private int targetColumn;

    private Piece pieceToAnimate;
    private Vector3 screenPositionToAnimate;

    private bool isInitialized;

    private void Start()
    {
        mainCamera = Camera.main;

        if (!InitializePuzzle())
        {
            enabled = false;
            return;
        }

        gameState = GameState.Start;
    }

    private bool InitializePuzzle()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Game: Main Camera not found.");
            return false;
        }

        if (go == null || go.Length != Constants.MaxSize)
        {
            Debug.LogError(
                $"Game: Exactly {Constants.MaxSize} puzzle pieces must be assigned."
            );
            return false;
        }

        if (Constants.MaxRows != 4 || Constants.MaxColumns != 4)
        {
            Debug.LogError("Game: Current puzzle setup expects a 4x4 grid.");
            return false;
        }

        pieceLookup.Clear();

        for (int i = 0; i < Constants.MaxRows; i++)
        {
            for (int j = 0; j < Constants.MaxColumns; j++)
            {
                int index = i * Constants.MaxColumns + j;
                GameObject pieceObject = go[index];

                if (pieceObject == null)
                {
                    Debug.LogError($"Game: Puzzle piece at index {index} is missing.");
                    return false;
                }

                pieceObject.SetActive(true);

                SpriteRenderer spriteRenderer =
                    pieceObject.GetComponent<SpriteRenderer>();

                if (spriteRenderer == null || spriteRenderer.sprite == null)
                {
                    Debug.LogError(
                        $"Game: SpriteRenderer or Sprite missing on {pieceObject.name}."
                    );
                    return false;
                }

                pieceObject.transform.position =
                    GetScreenCoordinatesFromViewport(i, j);

                Piece piece = new Piece
                {
                    GameObject = pieceObject,
                    OriginalI = i,
                    OriginalJ = j,
                    CurrentI = i,
                    CurrentJ = j
                };

                Matrix[i, j] = piece;
                pieceLookup[pieceObject] = piece;

                BoxCollider2D collider =
                    pieceObject.GetComponent<BoxCollider2D>();

                if (collider == null)
                    pieceObject.AddComponent<BoxCollider2D>();
            }
        }

        ScalePieces();

        // Select one random empty position.
        blankRow = Random.Range(0, Constants.MaxRows);
        blankColumn = Random.Range(0, Constants.MaxColumns);

        GameObject blankObject = go[
            blankRow * Constants.MaxColumns + blankColumn
        ];

        Matrix[blankRow, blankColumn] = null;
        pieceLookup.Remove(blankObject);
        blankObject.SetActive(false);

        isInitialized = true;
        return true;
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        switch (gameState)
        {
            case GameState.Start:
                if (Input.GetMouseButtonUp(0))
                {
                    Shuffle();
                    gameState = GameState.Playing;
                }
                break;

            case GameState.Playing:
                CheckPieceInput();
                break;

            case GameState.Animating:
                AnimateMovement();
                break;

            case GameState.End:
                if (Input.GetMouseButtonUp(0))
                    RestartGame();
                break;
        }
    }

    private void Shuffle()
    {
        // Shuffle through legal moves so the puzzle remains solvable.
        int previousBlankRow = -1;
        int previousBlankColumn = -1;

        const int shuffleMoves = 150;

        for (int move = 0; move < shuffleMoves; move++)
        {
            List<Vector2Int> availableMoves = GetAdjacentCells(
                blankRow,
                blankColumn
            );

            // Avoid immediately reversing the previous move.
            if (availableMoves.Count > 1)
            {
                availableMoves.RemoveAll(
                    cell => cell.x == previousBlankRow &&
                            cell.y == previousBlankColumn
                );
            }

            if (availableMoves.Count == 0)
                break;

            Vector2Int selectedCell =
                availableMoves[Random.Range(0, availableMoves.Count)];

            previousBlankRow = blankRow;
            previousBlankColumn = blankColumn;

            SwapCells(
                blankRow,
                blankColumn,
                selectedCell.x,
                selectedCell.y
            );

            blankRow = selectedCell.x;
            blankColumn = selectedCell.y;
        }

        // Prevent starting in an already solved state.
        if (IsPuzzleSolved())
        {
            List<Vector2Int> moves =
                GetAdjacentCells(blankRow, blankColumn);

            if (moves.Count > 0)
            {
                Vector2Int cell = moves[0];

                SwapCells(
                    blankRow,
                    blankColumn,
                    cell.x,
                    cell.y
                );

                blankRow = cell.x;
                blankColumn = cell.y;
            }
        }
    }

    private List<Vector2Int> GetAdjacentCells(int row, int column)
    {
        List<Vector2Int> cells = new List<Vector2Int>(4);

        if (row > 0)
            cells.Add(new Vector2Int(row - 1, column));

        if (row < Constants.MaxRows - 1)
            cells.Add(new Vector2Int(row + 1, column));

        if (column > 0)
            cells.Add(new Vector2Int(row, column - 1));

        if (column < Constants.MaxColumns - 1)
            cells.Add(new Vector2Int(row, column + 1));

        return cells;
    }

    private void CheckPieceInput()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        Vector3 mousePosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mousePosition);

        if (hit == null)
            return;

        GameObject clickedObject = hit.gameObject;

        if (!pieceLookup.TryGetValue(clickedObject, out Piece clickedPiece))
            return;

        int row = clickedPiece.CurrentI;
        int column = clickedPiece.CurrentJ;

        if (Mathf.Abs(row - blankRow) +
            Mathf.Abs(column - blankColumn) != 1)
        {
            return;
        }

        pieceToAnimate = clickedPiece;

        targetRow = blankRow;
        targetColumn = blankColumn;

        screenPositionToAnimate =
            GetScreenCoordinatesFromViewport(blankRow, blankColumn);

        gameState = GameState.Animating;
    }

    private void AnimateMovement()
    {
        if (pieceToAnimate == null ||
            pieceToAnimate.GameObject == null)
        {
            gameState = GameState.Playing;
            return;
        }

        Transform pieceTransform = pieceToAnimate.GameObject.transform;

        pieceTransform.position = Vector3.MoveTowards(
            pieceTransform.position,
            screenPositionToAnimate,
            Mathf.Max(0.01f, AnimSpeed) * Time.deltaTime
        );

        if (Vector3.Distance(
            pieceTransform.position,
            screenPositionToAnimate
        ) <= 0.01f)
        {
            CompleteMovement();
        }
    }

    private void CompleteMovement()
    {
        int oldRow = pieceToAnimate.CurrentI;
        int oldColumn = pieceToAnimate.CurrentJ;

        SwapCells(
            oldRow,
            oldColumn,
            targetRow,
            targetColumn
        );

        // The empty cell moves to the piece's previous position.
        blankRow = oldRow;
        blankColumn = oldColumn;

        pieceToAnimate = null;
        gameState = GameState.Playing;

        CheckForVictory();
    }

    private void SwapCells(
        int rowA,
        int columnA,
        int rowB,
        int columnB)
    {
        Piece temp = Matrix[rowA, columnA];

        Matrix[rowA, columnA] = Matrix[rowB, columnB];
        Matrix[rowB, columnB] = temp;

        UpdatePiecePosition(rowA, columnA);
        UpdatePiecePosition(rowB, columnB);
    }

    private void UpdatePiecePosition(int row, int column)
    {
        Piece piece = Matrix[row, column];

        if (piece == null)
            return;

        piece.CurrentI = row;
        piece.CurrentJ = column;

        piece.GameObject.transform.position =
            GetScreenCoordinatesFromViewport(row, column);
    }

    private bool IsPuzzleSolved()
    {
        for (int i = 0; i < Constants.MaxRows; i++)
        {
            for (int j = 0; j < Constants.MaxColumns; j++)
            {
                Piece piece = Matrix[i, j];

                if (piece == null)
                    continue;

                if (piece.CurrentI != piece.OriginalI ||
                    piece.CurrentJ != piece.OriginalJ)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void CheckForVictory()
    {
        if (!IsPuzzleSolved())
            return;

        gameState = GameState.End;

        StartCoroutine(ShowComingSoonAfterDelay());
    }

    private IEnumerator ShowComingSoonAfterDelay()
    {
        yield return new WaitForSeconds(
            Mathf.Max(0f, delayBeforePanel)
        );

        if (comingSoonPanel != null)
            comingSoonPanel.SetActive(true);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    private void ScalePieces()
    {
        SpriteRenderer spriteRenderer =
            go[0].GetComponent<SpriteRenderer>();

        float screenHeight = mainCamera.orthographicSize * 2f;
        float screenWidth =
            screenHeight * Screen.width / Screen.height;

        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        if (spriteWidth <= 0f || spriteHeight <= 0f)
            return;

        float width = screenWidth / spriteWidth / Constants.MaxColumns;
        float height = screenHeight / spriteHeight / Constants.MaxRows;

        Vector3 scale = new Vector3(width, height, 1f);

        foreach (GameObject piece in go)
        {
            if (piece != null)
                piece.transform.localScale = scale;
        }
    }

    private Vector3 GetScreenCoordinatesFromViewport(
        int row,
        int column)
    {
        float x = (column + 0.5f) / Constants.MaxColumns;
        float y = 1f - (row + 0.5f) / Constants.MaxRows;

        Vector3 point = mainCamera.ViewportToWorldPoint(
            new Vector3(x, y, mainCamera.nearClipPlane)
        );

        point.z = 0f;
        return point;
    }

    private void OnGUI()
    {
        if (gameState == GameState.Start)
        {
            GUI.Label(
                new Rect(20, 20, 250, 40),
                "Tap to start!"
            );
        }
        else if (gameState == GameState.End)
        {
            GUI.Label(
                new Rect(20, 20, 350, 40),
                "Puzzle completed! Tap to restart."
            );
        }
    }
}