using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;

public class GridController : MonoBehaviour
{
    //GridController to Handle Grid Operations
    [SerializeField] IGridGenerator grid;
    [SerializeField] GridManager gridManager;
    private CellClickHandler selectedCell;
    List<GameObject> matchingRowCells = new List<GameObject>();
    List<GameObject> matchingColCells = new List<GameObject>();
    List<GameObject> mergedCells = new List<GameObject>();
    int sameCountRow = 0;
    int sameCountCol = 0;
    private bool isCellSelected = false;

    private void OnEnable()
    {
        CellClickHandler.OnCellClick += HandleCellClick;
    }
    private void OnDisable()
    {
        CellClickHandler.OnCellClick -= HandleCellClick;
    }
    private void Start()
    {
        grid = GameObject.FindObjectOfType<GridGenerator>();
    }
    private void HandleFirstClick(CellClickHandler cell)
    {
        selectedCell = cell;
        isCellSelected = true;
    }
    private void HandleSecondClick(CellClickHandler cell)
    {
        Vector2 selectedCellGridPosition = selectedCell.GetGridPosition();
        Vector2 cellGridPosition = cell.GetGridPosition();
        if (IsAdjacent(selectedCell, cell))
        {
            gridManager.SwapCells(selectedCell.gameObject,cell.gameObject);
        }
        isCellSelected = false;
        selectedCell = null;
    }
    public void HandleCellClick(CellClickHandler cell)
    {
        if (!isCellSelected)
        {
            HandleFirstClick(cell);
            return;
        }
        HandleSecondClick(cell);
    }
    private bool IsAdjacent(CellClickHandler cell1, CellClickHandler cell2)
    {
        Debug.Log("IsAdjacent girdi");
        Vector2 position1 = cell1.GetGridPosition();
        Vector2 position2 = cell2.GetGridPosition();
        int distance = Mathf.Abs((int)position1.x - (int)position2.x) + Mathf.Abs((int)position1.y - (int)position2.y);
        return distance == 1;
    }
    public void GetAdjacentCells(GameObject cell)
    {
        string tempName = cell.name;
        GetSameRow(cell, tempName);
        GetSameCol(cell, tempName);
        mergedCells = matchingColCells.Concat(matchingRowCells).ToList();
        CheckForDestroy(cell, mergedCells);
        ResetCount();
        matchingRowCells.Clear();
        matchingColCells.Clear();
    }
    #region Finding Same Cell operations
    void GetSameRow(GameObject cell, string name)
    {     
        int col = (int)cell.GetComponent<CellClickHandler>().GetGridPosition().x;
        int row = (int)cell.GetComponent<CellClickHandler>().GetGridPosition().y;
        FindMatchingCells(cell, row + 1, grid.rows, 1, true);
        FindMatchingCells(cell, row - 1, -1, -1, true);
    }
    void GetSameCol(GameObject cell, string name)
    {
        int col = (int)cell.GetComponent<CellClickHandler>().GetGridPosition().x;
        int row = (int)cell.GetComponent<CellClickHandler>().GetGridPosition().y;
        FindMatchingCells(cell, col + 1, grid.columns, 1, false);
        FindMatchingCells(cell, col - 1, -1, -1, false);
    }
    private void FindMatchingCells(GameObject cell, int start, int end, int step, bool isRow)
    {
        string name = cell.name;
        int i = start;
        while (i != end)
        {
            int x = isRow ? (int)cell.GetComponent<CellClickHandler>().GetGridPosition().x : i;
            int y = isRow ? i : (int)cell.GetComponent<CellClickHandler>().GetGridPosition().y;
            GameObject otherCell = grid.GetInstantiatedPrefabs(x, y);
            if (otherCell != null && otherCell.name == name)
            {
                if (isRow)
                {
                    matchingRowCells.Add(otherCell);
                    sameCountRow++;
                }
                else
                {
                    matchingColCells.Add(otherCell);
                    sameCountCol++;
                }
                i += step;
            }
            else
            {
                break;
            }
        }
    }
    #endregion
    bool CanBeDestroyed()
    {
        if (sameCountCol  >= 2 || sameCountRow >= 2)
        {
            return true;
        }
        else return false;
    }
    void CheckForDestroy(GameObject cell, List<GameObject> list) // Need to Remove from instantiatedPrefabs as well by making it's value null
    {
        int col = (int)cell.GetComponent<CellClickHandler>().GetGridPosition().x;
        int row = (int)cell.GetComponent<CellClickHandler>().GetGridPosition().y;
        
        if (CanBeDestroyed())
        {
            list.Add(cell);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                GameObject go = list[i];
                Vector2Int gridPosition = new Vector2Int((int)go.GetComponent<CellClickHandler>().GetGridPosition().x, (int)go.GetComponent<CellClickHandler>().GetGridPosition().y);
                GameObject destroyedCell = grid.GetInstantiatedPrefabs(gridPosition.x, gridPosition.y);
                destroyedCell = null;
                Destroy(go);        
                list.RemoveAt(i);
            }
        }        
    }
    public void SwapCellsInGrid(int x1, int y1, int x2, int y2)
    {
        GameObject temp = grid.GetInstantiatedPrefabs(x1, y1);
        grid.SetInstantiatedPrefabs(x1, y1, grid.GetInstantiatedPrefabs(x2, y2));
        grid.SetInstantiatedPrefabs(x2, y2, temp);
    }
    public void CheckAndHandleFalling()
    {
        bool cellsMoved = true;
        while (cellsMoved)
        { 
            cellsMoved = false;     
            for (int col = 0; col < grid.columns; col++)
            {
                for (int row = 0; row < grid.rows; row++)
                {
                    if (grid.GetInstantiatedPrefabs(col, row) == null)
                    {
                        int lowestEmptyRow = FindLowestEmptySpace(col, row);
                        for (int aboveRow = row + 1; aboveRow < grid.rows; aboveRow++)
                        {
                            if (grid.GetInstantiatedPrefabs(col, aboveRow) != null)
                            {
                                cellsMoved = true;
                                MoveCellDown(aboveRow, col, lowestEmptyRow);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    void MoveCellDown(int sourceRow, int col, int targetRow)
    {
        GameObject sourceCell = grid.GetInstantiatedPrefabs(col, sourceRow);
        grid.SetInstantiatedPrefabs(col, sourceRow, null);
        grid.SetInstantiatedPrefabs(col, targetRow, sourceCell);
        GameObject targetCell = grid.GetInstantiatedPrefabs(col, targetRow);
        Vector2 targetPosition = targetCell.transform.position;
        targetPosition.y -= grid.CellSize.y * (sourceRow - targetRow);
        targetCell.transform.DOMove(targetPosition, 0.5f);
        targetCell.GetComponent<CellClickHandler>().SetGridPosition(col, targetRow);
    }
    private int FindLowestEmptySpace(int col, int startRow)
    {
        int tempMin = 10;
        for (int row = startRow; row < grid.rows; row++)
        {
            if (grid.GetInstantiatedPrefabs(col, row) == null)
            {
                if(row < tempMin)
                {
                    tempMin = row;
                }                
            }   
        }
        return tempMin;
    }
    void ResetCount()
    {
        sameCountCol = sameCountRow = 0;
        mergedCells.Clear();
    }
}

