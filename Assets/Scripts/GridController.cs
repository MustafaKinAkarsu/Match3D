using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] CellClickHandler clickHandler;
    [SerializeField] GridGenerator gridGenerator;
    private CellClickHandler selectedCell;
    List<GameObject> matchingRowCells = new List<GameObject>();
    List<GameObject> matchingColCells = new List<GameObject>();
    int sameCountRowAbove = 0;
    int sameCountRowBelow = 0;
    int sameCountColLeft = 0;
    int sameCountColRight = 0;
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
            isCellSelected = true;
        }
        else
        {
            if (IsAdjacent(selectedCell, cell))
            {
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

        GameObject temp = gridGenerator.instantiatedPrefabs[(int)gridPosition1.x, (int)gridPosition1.y];
        gridGenerator.instantiatedPrefabs[(int)gridPosition1.x, (int)gridPosition1.y] = gridGenerator.instantiatedPrefabs[(int)gridPosition2.x, (int)gridPosition2.y];
        gridGenerator.instantiatedPrefabs[(int)gridPosition2.x, (int)gridPosition2.y] = temp;


        Vector3 tempPosition = cell1.transform.position;
        cell1.transform.position = cell2.transform.position;
        cell2.transform.position = tempPosition;

        cell1.SetGridPosition((int)cell1.transform.position.x, (int)cell1.transform.position.y);

        cell2.SetGridPosition((int)cell2.transform.position.x, (int)cell2.transform.position.y);


        GetAdjacentCells((int)cell1.GetGridPosition().x, (int)cell1.GetGridPosition().y);
        Debug.Log("-----------------------");
        GetAdjacentCells((int)cell2.GetGridPosition().x, (int)cell2.GetGridPosition().y);
    }

    public void GetAdjacentCells(int row, int col)
    {

        string tempName = gridGenerator.instantiatedPrefabs[row, col].name;

        

        if (row == 0) GetSameRowAbove(row, col, tempName);
        else if (col == 0) GetSameColRight(row, col, tempName);
        else if (col == gridGenerator.columns) GetSameColLeft(row, col, tempName);
        else if (row == gridGenerator.rows) GetSameRowBelow(row, col, tempName);
        else
        {
            GetSameRowAbove(row, col, tempName);

            GetSameColLeft(row, col, tempName);

            GetSameColRight(row, col, tempName);

            GetSameRowBelow(row, col, tempName);
            for (int i = matchingColCells.Count - 1; i >= 0; i--)
            {
                GameObject go = matchingColCells[i];
                Debug.Log("Object : " + go.GetComponent<CellClickHandler>());
                DestroyTile(go, gridGenerator.instantiatedPrefabs[row, col]);
                matchingColCells.RemoveAt(i);
            }
        }
        ResetCount();
    }
    #region Finding Same Cell operations

    void GetSameRowAbove(int row, int col, string name)
    {
        for (int i = row + 1; i < gridGenerator.QuadSize.y; i++)
        {
            if (gridGenerator.instantiatedPrefabs[i, col].name.Equals(name)) 
            {
                sameCountRowAbove++;
                matchingRowCells.Add(gridGenerator.instantiatedPrefabs[i, col]);
            } 
            
            else break;
        }
        Debug.Log("Same Row Count Above : " + sameCountRowAbove);
    }
    void GetSameRowBelow(int row, int col, string name)
    {
        for (int i = row - 1; i >= 0; i--)
        {
            if (gridGenerator.instantiatedPrefabs[i, col].name.Equals(name)) 
            {
                sameCountRowBelow++;
                matchingRowCells.Add(gridGenerator.instantiatedPrefabs[i, col]);
            } 
            else break;
        }
        Debug.Log("Same Row Count Below : " + sameCountRowBelow);
    }
    void GetSameColLeft(int row, int col, string name)
    {
        for (int i = col - 1; i >= 0; i--)
        {
            if (gridGenerator.instantiatedPrefabs[row, i].name.Equals(name)) 
            {
                sameCountColLeft++;
                matchingColCells.Add(gridGenerator.instantiatedPrefabs[row, i]);
            }
            else break;
        }
        Debug.Log("Same Col Count Left : " + sameCountColLeft); 
    }
    void GetSameColRight(int row, int col, string name)
    {
        for (int i = col + 1; i < gridGenerator.QuadSize.x; i++)
        {
            if (gridGenerator.instantiatedPrefabs[row, i].name.Equals(name)) 
            {
                sameCountColRight++;
                matchingColCells.Add(gridGenerator.instantiatedPrefabs[row, i]);
            } 
            else break;
        }
        Debug.Log("Same Col Count Right : " + sameCountColRight);
    }
    #endregion
    void ResetCount()
    {
        sameCountRowAbove = sameCountRowBelow = sameCountColLeft = sameCountColRight = 0;
    }
    bool CanBeDestroyed()
    {
        if (sameCountColLeft + sameCountColRight >= 2 || sameCountRowAbove + sameCountRowBelow >= 2)
        {
            return true;
        }
        else return false;

    }
    void DestroyTile(GameObject cell, GameObject currentCell)
    {
        if (CanBeDestroyed())
        {
            Destroy(cell);
            if(currentCell != null) Destroy(currentCell);
        }
    }
}

