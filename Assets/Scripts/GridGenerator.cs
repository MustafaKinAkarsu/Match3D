using UnityEngine;
using Unity;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
    public GameObject quad; // Reference to your quad
    public int rows = 5;
    public int columns = 5;
    private Vector3 startPosition;
    [SerializeField] GameObject[] gemPrefabs;
    int previousRandom;
    public GameObject[,] instantiatedPrefabs;
    Vector3 quadSize;
    Vector3 cellSize;
    public Vector3 CellSize
    {
        get { return cellSize; }
        set { cellSize = value; }
    }


    void Start()
    {
        startPosition = quad.GetComponent<Renderer>().bounds.min;
        instantiatedPrefabs = new GameObject[rows, columns];
        CreateGrid();
    }

    void CreateGrid()
    {
        //instantiatedPrefabs = new GameObject[rows, columns];
        quadSize = quad.GetComponent<Renderer>().bounds.size;
        CellSize = new Vector3(quadSize.x / columns, quadSize.y / rows, 1f);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {

                 int random;
                 do
                 {
                     random = Random.Range(0, 4);
                 }
                 while (random == previousRandom);
                previousRandom = random;
                float xPos = (col * cellSize.x + cellSize.x / 2) + startPosition.x;
                float yPos = (row * cellSize.y + cellSize.y / 2) + startPosition.y;
                Vector3 position = new Vector3(xPos, yPos, 0f);
                GameObject cell = Instantiate(gemPrefabs[random], position, Quaternion.identity);
                //Debug.Log("Cell pos : " + cell.transform.position);
                cell.transform.parent = transform;
                instantiatedPrefabs[row, col] = cell;
                //instantiatedCells.Add(cell);
            }
        }
    }
    public GameObject GetCell(int row, int col)
    {
        if (row >= 0 && row < rows && col >= 0 && col < columns)
        {
            return instantiatedPrefabs[row, col];
        }
        else
        {
            Debug.LogError("Row or column index out of bounds.");
            return null;
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
    

}