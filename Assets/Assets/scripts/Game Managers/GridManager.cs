using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public bool generatedFirstGrid;
    public int minDimensionsSize;
    public int maxDimensionsSize;
    public int generateWidth;
    public int generateHeight;
    public int width;
    public int height;
    public float cellSize;
    public int gridResolution;
    public string[,] cells;
    private GameObject firstGridCell;
    private GameObject lastGridCell;
    public GameObject cellPrefab;
    public GameObject[,] grid;
    public OrbitCamera orbitCameraScript;

    [Header("3D Text Settings")]
    public GameObject threeDTextPrefab;
    public float textHeightOffset = 1f;

    [Header("Screens and Text")]
    public GameObject selectSizeScreen;
    public TextMeshProUGUI widthText;
    public TextMeshProUGUI heightText;

    public void GridScreenStatus(bool isOn)
    {
        if (generatedFirstGrid || isOn)
        {
            selectSizeScreen.SetActive(isOn);
        }
    }

    public void ConfirmGridSize()
    {
        //once done we generate the grid
        GenerateGrid();
    }

    bool IsCellFree(int x, int y)
    {
        //check if the cell is free by checking the symbol
        if (x < 0 || y < 0 || x >= width || y >= height) return false;
        string cell = cells[x, y];
        return cell == "." || cell == "[ . ]";
    }

    void SetCell(int x, int y, string symbol)
    {
        //set the cell to the symbol of the object
        cells[x, y] = "[ " + symbol + " ]";
    }

    public bool CanPlace(int x, int y, int w, int h)
    {
        //check if the objects are able to placed by checking if the cell is free
        for (int incrementX = 0; incrementX < w; incrementX++)
            for (int incrementY = 0; incrementY < h; incrementY++)
                if (!IsCellFree(x + incrementX, y + incrementY)) return false;

        return true;
    }

    public void Fill(int x, int y, int w, int h, string symbol, string rotation)
    {
        string token;

        token = symbol.ToString();

        //fill the cells with the symbol of the object
        for (int ix = 0; ix < w; ix++)
            for (int iy = 0; iy < h; iy++)
                SetCell(x + ix, y + iy, token);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        //calculate the local pos of the world point
        Vector3 localPos = worldPos - transform.position;

        int x = Mathf.RoundToInt(localPos.x + width / 2f - 0.5f);
        int y = Mathf.RoundToInt(localPos.z + height / 2f - 0.5f);

        //return the grid cords as a vector2int
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        //calculate the world pos of the cell based on its grid cords and cell size
        float offsetX = (width * cellSize) / 2f - cellSize / 2f;
        float offsetZ = (height * cellSize) / 2f - cellSize / 2f;

        Vector3 position = new Vector3(
            x * cellSize - offsetX,
            0,
            y * cellSize - offsetZ
        ) + transform.position;

        //return the world pos of the cell
        return position;
    }

    public Vector3 GetCenterWorld(Vector3 bottomLeftCellWorld, int w, int h, float cellSize)
    {
        //find the center world pos of the object
        return bottomLeftCellWorld + new Vector3((w * cellSize) / 2f - cellSize / 2f, 0f, (h * cellSize) / 2f - cellSize / 2f);
    }

    public void GenerateGrid()
    {
        //set to true so menu can be closed to create a new one
        generatedFirstGrid = true;

        //clear the previous grid if it exists
        ClearGrid();

        //sets the grid and height to the selected number
        width = generateWidth;
        height = generateHeight;

        cells = new string[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                SetCell(x, y, ".");

        grid = new GameObject[width, height];

        //generating the grid cells
        for (int x = 0; x < width; x++)
        {
            //calculate the world pos of the cell based on its grid cords and cell size
            for (int y = 0; y < height; y++)
            {
                float offsetX = (width * cellSize) / 2f - cellSize / 2f;
                float offsetZ = (height * cellSize) / 2f - cellSize / 2f;
                Vector3 position = new Vector3(
                    x * cellSize - offsetX,
                    0,
                    y * cellSize - offsetZ
                ) + transform.position;

                //instantiate the grid cell prefab
                GameObject cell = Instantiate(cellPrefab, position, cellPrefab.transform.rotation, transform);
                grid[x, y] = cell;
            }
        }
        //find the first and last cell
        if (width > 0 && height > 0)
        {
            int centerX = (width - 1) / 2;
            firstGridCell = grid[centerX, height - 1];
            lastGridCell = grid[centerX, 0];
        }

        //generating top text at the first cell
        GameObject topTextInstance = Instantiate(threeDTextPrefab,
            new Vector3(transform.position.x, transform.position.y, firstGridCell.transform.position.z + textHeightOffset),
            threeDTextPrefab.transform.rotation, transform);

        TextMeshPro topTextMesh = topTextInstance.GetComponent<TextMeshPro>();
        topTextMesh.text = "Top";
        //scaling font size based on cell size
        topTextMesh.fontSize = topTextMesh.fontSize * height / 2;
        if (topTextMesh.fontSize > 15) topTextMesh.fontSize = 15;

        //generating bottom text at the last cell
        GameObject bottomTextInstance = Instantiate(threeDTextPrefab,
            new Vector3(transform.position.x, transform.position.y, lastGridCell.transform.position.z - textHeightOffset),
            threeDTextPrefab.transform.rotation, transform);

        TextMeshPro bottomTextMesh = bottomTextInstance.GetComponent<TextMeshPro>();
        bottomTextMesh.text = "Bottom";
        bottomTextMesh.fontSize = bottomTextMesh.fontSize * height / 2;
        if (bottomTextMesh.fontSize > 15) bottomTextMesh.fontSize = 15;

        GameObject[] existingFurniture = GameObject.FindGameObjectsWithTag("Furniture");
        foreach (GameObject obj in existingFurniture)
        {
            Destroy(obj);
        }

        UIManager.currentState = "main";

        GridScreenStatus(false);
        orbitCameraScript.ResetCamOrbit();

    }

    void ClearGrid()
    {
        //destroy all child objects
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public string ToAscii()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //starting from the top row first, output the grid
        for (int y = height - 1; y >= 0; y--)
        {
            //output each cell in the row
            for (int x = 0; x < width; x++)
            {
                sb.Append(cells[x, y]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }

        //return the ascii as a string
        return sb.ToString();
    }

    //when selecting the width and height
    public void WidthAdjustButton(int increment)
    {
        if (increment > 0 && generateWidth < maxDimensionsSize)
        {
            generateWidth += increment;
        }
        else if (increment < 0 && generateWidth > minDimensionsSize)
        {
            generateWidth += increment;
        }

        widthText.text = generateWidth.ToString();
    }

    public void HeightAdjustButton(int increment)
    {
        if (increment > 0 && generateHeight < maxDimensionsSize)
        {
            generateHeight += increment;
        }
        else if (increment < 0 && generateHeight > minDimensionsSize)
        {
            generateHeight += increment;
        }

        heightText.text = generateHeight.ToString();
    }
}
