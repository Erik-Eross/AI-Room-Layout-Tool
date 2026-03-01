using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public string[,] cells;

    public GameObject cellPrefab;
    private GameObject[,] grid;

    void Start()
    {
        cells = new string[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                cells[x, y] = ".";
        GenerateGrid();
    }

    bool IsCellFree(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return false;
        return cells[x, y] == ".";
    }

    void SetCell(int x, int y, string symbol)
    {
        cells[x, y] = symbol;
    }

    public bool CanPlace(int x, int y, int w, int h)
    {
        for (int incrementX = 0; incrementX < w; incrementX++)
            for (int incrementY = 0; incrementY < h; incrementY++)
                if (!IsCellFree(x + incrementX, y + incrementY)) return false;

        return true;
    }

    public void Fill(int x, int y, int w, int h, char symbol, float rotation)
    {
        string token;
        if(rotation == 0) { token = symbol.ToString(); }
        else{ token = symbol + rotation.ToString(); }

        for (int ix = 0; ix < w; ix++)
            for (int iy = 0; iy < h; iy++)
                SetCell(x + ix, y + iy, token);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;

        int x = Mathf.RoundToInt(localPos.x + width / 2f - 0.5f);
        int y = Mathf.RoundToInt(localPos.z + height / 2f - 0.5f);

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        float offsetX = (width * cellSize) / 2f - cellSize / 2f;
        float offsetZ = (height * cellSize) / 2f - cellSize / 2f;
        Vector3 position = new Vector3(
            x * cellSize - offsetX,
            0,
            y * cellSize - offsetZ
        ) + transform.position;

        return position;
    }

    public Vector3 GetCenterWorld(Vector3 bottomLeftCellWorld, int w, int h, float cellSize)
    {
        return bottomLeftCellWorld + new Vector3((w * cellSize) / 2f - cellSize / 2f, 0f, (h * cellSize) / 2f - cellSize / 2f);
    }

    void GenerateGrid()
    {
        ClearGrid();

        grid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float offsetX = (width * cellSize) / 2f - cellSize / 2f;
                float offsetZ = (height * cellSize) / 2f - cellSize / 2f;
                Vector3 position = new Vector3(
                    x * cellSize - offsetX,
                    0,
                    y * cellSize - offsetZ
                ) + transform.position;

                GameObject cell = Instantiate(cellPrefab, position, cellPrefab.transform.rotation, transform);
                grid[x, y] = cell;
            }
        }
    }

    void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public string ToAscii()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int y = height - 1; y >= 0; y--) // top row first
        {
            for (int x = 0; x < width; x++)
            {
                sb.Append(cells[x, y]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
