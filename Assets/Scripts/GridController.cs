using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] CellClickHandler clickHandler;
    [SerializeField] GridGenerator gridGenerator;

    private void OnEnable()
    {
        CellClickHandler.OnCellClick += HandleCellClick;
    }

    private void OnDisable()
    {
        CellClickHandler.OnCellClick -= HandleCellClick;
    }

    private void HandleCellClick(CellClickHandler cell)
    {
        Vector2 gridPosition = cell.GetGridPosition();
        GetAdjacentCells((int)gridPosition.y, (int)gridPosition.x);
    }

    public List<GameObject> GetAdjacentCells(int row, int col)
    {
        List<GameObject> adjacentCells = new List<GameObject>();

        // Check the cell above
        if (row > 0)
        {
            adjacentCells.Add(gridGenerator.instantiatedPrefabs[row - 1, col]);
        }

        // Check the cell below
        if (row < gridGenerator.rows - 1)
        {
            adjacentCells.Add(gridGenerator.instantiatedPrefabs[row + 1, col]);
        }

        // Check the cell to the left
        if (col > 0)
        {
            adjacentCells.Add(gridGenerator.instantiatedPrefabs[row, col - 1]);
        }

        // Check the cell to the right
        if (col < gridGenerator.columns - 1)
        {
            adjacentCells.Add(gridGenerator.instantiatedPrefabs[row, col + 1]);
        }

        foreach (GameObject go in adjacentCells)
        {
            Debug.Log(go);
        }
        return adjacentCells;
    }
}
