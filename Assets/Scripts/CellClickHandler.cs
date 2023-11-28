using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class CellClickHandler : MonoBehaviour
{
    private GridGenerator gridGenerator; 
    [SerializeField] int column, row;
    public string TileType; 
    public static Action<CellClickHandler> OnCellClick;

    private void Start()
    {
        gridGenerator = GridGenerator.Instance;
        SetGridPosition((int)this.transform.position.x, (int)this.transform.position.y);
    }

    public void SetGridPosition(int Col, int Row)
    {
        Vector3 cellPosition = new Vector3(Col, Row, 0f);
        column = Mathf.FloorToInt((cellPosition.x - gridGenerator.GetStartPosition().x) / gridGenerator.GetCellSize().x);
        row = Mathf.FloorToInt((cellPosition.y - gridGenerator.GetStartPosition().y) / gridGenerator.GetCellSize().y);
    }
    public Vector2 GetGridPosition()
    {
        Vector2 gridPosition = new Vector2(column, row);
        return gridPosition;
    }
    private void OnMouseDown()
    {
        Vector2 gridPos = GetGridPosition();
        Debug.Log("Coordinates  " + "col : " + gridPos.x + " row " + gridPos.y);
        OnCellClick?.Invoke(this);
    }
}
