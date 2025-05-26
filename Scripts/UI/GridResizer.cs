using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridResizer : MonoBehaviour
{
    public GridLayoutGroup grid;
    public int columnCount = 2;
    public float spacing = 20f;
    public float padding = 40f;

    void Start()
    {
        Resize();
    }

    void Resize()
    {
        // Canvas 스케일 기준으로 UI 해상도 적용
        Canvas canvas = GetComponentInParent<Canvas>();
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        
        float referenceWidth = scaler.referenceResolution.x;
        float screenWidth = Screen.width;

        // 현재 스케일 적용
        float scaleFactor = canvas.scaleFactor;
        float adjustedWidth = screenWidth / scaleFactor;

        float totalSpacing = spacing * (columnCount - 1);
        float totalPadding = padding * 2;

        float cellWidth = (adjustedWidth - totalSpacing - totalPadding) / columnCount;
        grid.cellSize = new Vector2(cellWidth, cellWidth);
        grid.spacing = new Vector2(spacing, spacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columnCount;
    }
}
