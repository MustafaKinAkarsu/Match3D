using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] CellClickHandler clickHandler;
    [SerializeField] GridGenerator gridGenerator;
    private CellClickHandler selectedCell;
    private bool isCellSelected = false;

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
        if (!isCellSelected)
        {
            selectedCell = cell;
            Debug.Log("Selected Cell : " + selectedCell + " Position : " + selectedCell.GetGridPosition());
            isCellSelected = true;
        }
        else
        {
            if (IsAdjacent(selectedCell, cell))
            {
                Debug.Log("Swapping with : " + cell + " Position : " + cell.GetGridPosition());
                SwapCells(selectedCell, cell);
            }
            isCellSelected = false;
        }
    }

    private bool IsAdjacent(CellClickHandler cell1, CellClickHandler cell2)
    {
        int row1 = (int)cell1.GetGridPosition().y;
        int col1 = (int)cell1.GetGridPosition().x;

        int row2 = (int)cell2.GetGridPosition().y;
        int col2 = (int)cell2.GetGridPosition().x;

        int rowDiff = Mathf.Abs(row1 - row2);
        int colDiff = Mathf.Abs(col1 - col2);

        return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
    }

    private void SwapCells(CellClickHandler cell1, CellClickHandler cell2)
    {
        Vector2 gridPosition1 = cell1.GetGridPosition();
        Vector2 gridPosition2 = cell2.GetGridPosition();

        GameObject temp = gridGenerator.instantiatedPrefabs[(int)gridPosition1.y, (int)gridPosition1.x];
        gridGenerator.instantiatedPrefabs[(int)gridPosition1.y, (int)gridPosition1.x] = gridGenerator.instantiatedPrefabs[(int)gridPosition2.y, (int)gridPosition2.x];
        gridGenerator.instantiatedPrefabs[(int)gridPosition2.y, (int)gridPosition2.x] = temp;

        Vector3 tempPosition = cell1.transform.position;
        cell1.transform.position = cell2.transform.position;
        cell2.transform.position = tempPosition;

        cell1.SetGridPosition((int)cell1.transform.position.x, (int)cell1.transform.position.y);

        cell2.SetGridPosition((int)cell2.transform.position.x, (int)cell2.transform.position.y);
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
