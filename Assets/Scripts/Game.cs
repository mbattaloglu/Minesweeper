using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Game Settings")]
    public int width = 16;
    public int height = 16;
    public int minesCount = 32;

    private Board board;

    private Cell[][] state;

    private bool gameOver = false;

    private void OnValidate()
    {
        minesCount = Mathf.Clamp(minesCount, 0, width * height);
    }

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }

        else if (!gameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }

            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }
        }

    }

    public void NewGame()
    {
        gameOver = false;
        state = new Cell[width][];
        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);
        board.Draw(state);
    }

    private void Reveal()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(mousePosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == CellType.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }

        switch (cell.type)
        {
            case CellType.Mine:
                Explode(cell);
                break;
            case CellType.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.revealed = true;
                state[cellPosition.x][cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }
        board.Draw(state);
    }

    private void Flag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(mousePosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == CellType.Invalid || cell.revealed)
        {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x][cellPosition.y] = cell;
        board.Draw(state);
    }

    private void Flood(Cell cell)
    {
        if (cell.revealed)
        {
            return;
        }

        if (cell.type == CellType.Invalid || cell.type == CellType.Mine)
        {
            return;
        }

        cell.revealed = true;
        state[cell.position.x][cell.position.y] = cell;

        if (cell.type == CellType.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
    }

    private void Explode(Cell cell)
    {
        Debug.Log("Boom!");
        gameOver = true;

        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x][cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x][y];
                if (cell.type == CellType.Mine && !cell.flagged)
                {
                    cell.revealed = true;
                    state[x][y] = cell;
                }
            }
        }
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x][y];
                if (cell.type != CellType.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }

        Debug.Log("You win!");
        gameOver = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x][y];
                if (cell.type == CellType.Mine)
                {
                    cell.flagged = true;
                    state[x][y] = cell;
                }
            }
        }
    }

    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y))
        {
            return state[x][y];
        }

        return new Cell() { type = CellType.Invalid };
    }

    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            state[x] = new Cell[height];
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = CellType.Empty;
                state[x][y] = cell;
            }
        }
    }

    private void GenerateMines()
    {
        for (int i = 0; i < minesCount; i++)
        {
            int x, y;
            do
            {
                x = Random.Range(0, width);
                y = Random.Range(0, height);
            } while (state[x][y].type == CellType.Mine);

            state[x][y].type = CellType.Mine;
        }
    }

    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x][y];
                if (cell.type == CellType.Mine)
                {
                    continue;
                }

                cell.number = CountMines(x, y);
                if (cell.number > 0)
                {
                    cell.type = CellType.Number;
                }

                state[x][y] = cell;

            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int count = 0;

        for (int adjecentX = -1; adjecentX <= 1; adjecentX++)
        {
            for (int adjecentY = -1; adjecentY <= 1; adjecentY++)
            {
                if (adjecentX == 0 && adjecentY == 0)
                {
                    continue;
                }

                int x = cellX + adjecentX;
                int y = cellY + adjecentY;

                if (GetCell(x, y).type == CellType.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
