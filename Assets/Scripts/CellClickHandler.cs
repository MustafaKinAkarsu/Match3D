using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CellClickHandler : MonoBehaviour
{
    private GridGenerator gridManager; // Reference to the GridManager
    public int column, row;
    Vector2 currentPosition;
    public static event Action<CellClickHandler> OnCellClick;
    public Vector2 CurrentPosition
    {
        get { return currentPosition; }
        set { currentPosition = value; }
    }

    private void Start()
    {
        // Find the GridManager in the scene
        gridManager = FindObjectOfType<GridGenerator>();
        SetGridPosition();
    }

    void SetGridPosition()
    {
        // Calculate the column and row indices based on the cell's position
        Vector3 cellPosition = transform.position;
        column = Mathf.FloorToInt((cellPosition.x - gridManager.GetStartPosition().x) / gridManager.GetCellSize().x);
        row = Mathf.FloorToInt((cellPosition.y - gridManager.GetStartPosition().y) / gridManager.GetCellSize().y);
    }
    public Vector2 GetGridPosition()
    {
        Vector2 gridPosition = new Vector2(column, row);
        return gridPosition;
    }
  
    private void OnMouseDown()
    {
        CurrentPosition = GetGridPosition();
        OnCellClick?.Invoke(this);
    }
}
