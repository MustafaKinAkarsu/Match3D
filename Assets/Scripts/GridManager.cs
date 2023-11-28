using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Runtime.CompilerServices;

public class GridManager : MonoBehaviour
{
    [SerializeField] GridController _gridController;
    GridGenerator grid;

    public static GridManager Instance { get; private set; }
    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        grid = GameObject.FindObjectOfType<GridGenerator>();
    }  
    public void SwapCells(GameObject currentCell, GameObject swappedCell)
    {
        Vector2 currentGridPosition = currentCell.GetComponent<CellClickHandler>().GetGridPosition();
        Vector2 swappedGridPosition = swappedCell.GetComponent<CellClickHandler>().GetGridPosition();
        _gridController.SwapCellsInGrid((int)currentGridPosition.x, (int)currentGridPosition.y, (int)swappedGridPosition.x, (int)swappedGridPosition.y);
        StartCoroutine(MoveTile(currentCell, swappedCell));
    }

    IEnumerator MoveTile(GameObject currentCell, GameObject swappedCell)
    {      
        currentCell.transform.DOMove(swappedCell.transform.position, 0.5f);
        swappedCell.transform.DOMove(currentCell.transform.position, 0.5f);
        currentCell.GetComponent<CellClickHandler>().SetGridPosition((int)swappedCell.transform.position.x, (int)swappedCell.transform.position.y);
        swappedCell.GetComponent<CellClickHandler>().SetGridPosition((int)currentCell.transform.position.x, (int)currentCell.transform.position.y);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AdjacentCheck(currentCell, swappedCell));
    }
    void CheckAdjacent(GameObject cell)
    {
        _gridController.GetAdjacentCells(cell);
    }
    IEnumerator AdjacentCheck(GameObject go, GameObject gg)
    {
        CheckAdjacent(go);
        yield return null;
        StartCoroutine(CheckSwappedAdjacent(gg));
    }
    IEnumerator CheckSwappedAdjacent(GameObject go)
    {
        CheckAdjacent(go);
        yield return null;
        StartCoroutine(HandleFall());
    }
    IEnumerator HandleFall()
    {
        _gridController.CheckAndHandleFalling();
        yield return new WaitForSeconds(3f);
        StartCoroutine(FillBlankSpot());
    }
    IEnumerator FillBlankSpot()
    {
        grid.FillEmptyGridSpots();
        yield return null;
    }
}
