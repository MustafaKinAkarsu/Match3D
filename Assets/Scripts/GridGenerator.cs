using UnityEngine;
using Unity;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour, IGridGenerator
{
    public GameObject quad;
    [SerializeField]
    private int _rows = 5;
    public int rows { get { return _rows; } }
    [SerializeField]
    private int _columns = 5;
    public int columns { get { return _columns; } }
    private Vector3 startPosition;
    [SerializeField] GameObject[] gemPrefabs;
    int previousRandom = 0;
    public GameObject[,] instantiatedPrefabs;
    Vector3 quadSize;
    Vector3 cellSize;
    public Vector3 CellSize
    {
        get { return cellSize; }
        set { cellSize = value; }
    }
    public Vector3 QuadSize
    {
        get { return quadSize; }
        set { quadSize = value; }
    }
    public static GridGenerator Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        startPosition = quad.GetComponent<Renderer>().bounds.min;        
        CreateGrid();     
    }
    public void CreateGrid()
    {
        QuadSize = quad.GetComponent<Renderer>().bounds.size;
        CellSize = new Vector3(quadSize.x / columns, quadSize.y / rows, 1f);
        instantiatedPrefabs = new GameObject[columns, rows];
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                GameObject prefab;
                string tileType;
                 int random;
                 do
                 {
                     random = Random.Range(0, gemPrefabs.Length);
                     prefab = gemPrefabs[random];
                     tileType = prefab.GetComponent<CellClickHandler>().TileType;
                }
                 while (HasTwoMatching(row,col,tileType));
                previousRandom = random;
                float xPos = (col * CellSize.x + CellSize.x / 2) + startPosition.x; // startPosition.x = -2.5
                float yPos = (row * CellSize.y + CellSize.y / 2) + startPosition.y; // startPosition.y = -2.5
                Vector3 position = new Vector3(xPos, yPos, 0f);
                GameObject cell = Instantiate(prefab, position, Quaternion.identity);
                cell.transform.parent = transform;
                instantiatedPrefabs[col, row] = cell;              
            }
        }
    }
    public Vector3 GetStartPosition()
    {
        return quad.transform.position - quadSize / 2f;
    }
    public Vector3 GetCellSize()
    {
        return cellSize;
    }   
    private bool HasTwoMatching(int row, int col, string tileType)
    {
        string leftCell1 = col >= 1 ? instantiatedPrefabs[col - 1, row].GetComponent<CellClickHandler>().TileType : null;
        string leftCell2 = col >= 2 ? instantiatedPrefabs[col - 2, row].GetComponent<CellClickHandler>().TileType : null;
        string aboveCell1 = row >= 1 ? instantiatedPrefabs[col, row - 1].GetComponent<CellClickHandler>().TileType : null;
        string aboveCell2 = row >= 2 ? instantiatedPrefabs[col, row - 2].GetComponent<CellClickHandler>().TileType : null;

        if ((leftCell1 != null && leftCell2 != null && leftCell1 == tileType && leftCell2 == tileType)
            || (aboveCell1 != null && aboveCell2 != null && aboveCell1 == tileType && aboveCell2 == tileType))
            return true;

        return false;
    }
    public GameObject GetInstantiatedPrefabs(int col, int row)
    {
        return instantiatedPrefabs[col, row];
    }
    public void SetInstantiatedPrefabs(int col, int row, GameObject cell)
    {
        instantiatedPrefabs[col, row] = cell;
    }
    public void FillEmptyGridSpots()
    {
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (instantiatedPrefabs[col, row] == null)
                {
                    GameObject prefab;
                    string tileType;
                    int random;
                    do
                    {
                        random = Random.Range(0, gemPrefabs.Length);
                        prefab = gemPrefabs[random];
                        tileType = prefab.GetComponent<CellClickHandler>().TileType;
                    }
                    while (HasTwoMatching(row, col, tileType));

                    float xPos = (col * CellSize.x + CellSize.x / 2) + startPosition.x;
                    float yPos = (row * CellSize.y + CellSize.y / 2) + startPosition.y;
                    Vector3 position = new Vector3(xPos, yPos, 0f);
                    GameObject cell = Instantiate(prefab, position, Quaternion.identity);
                    cell.transform.parent = transform;
                    instantiatedPrefabs[col, row] = cell;
                }
            }
        }
    }
}
public interface IGridGenerator
{
    void CreateGrid();
    GameObject GetInstantiatedPrefabs(int col, int row);
    void SetInstantiatedPrefabs(int col, int row, GameObject cell);
    int rows { get; }
    int columns { get; }
    Vector3 CellSize { get; }
    // More methods...
}