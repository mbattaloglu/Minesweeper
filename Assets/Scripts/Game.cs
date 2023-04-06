using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Game Settings")]
    public int width = 16;
    public int height = 16;
    public int minesCount = 32;

    private Board board;

    private Cell[][] state;

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        state = new Cell[width][];
        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);
        board.Draw(state);
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
                int x = cellX + adjecentX;
                int y = cellY + adjecentY;

                if (adjecentX == 0 && adjecentY == 0)
                {
                    continue;
                }

                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    continue;
                }

                if (state[x][y].type == CellType.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
