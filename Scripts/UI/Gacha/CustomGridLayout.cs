using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GridLayoutGroup처럼 자식 RectTransform을 그리드 형태로 정렬하는 커스텀 레이아웃 컴포넌트입니다.
/// 자식 오브젝트는 Pivot이 (0.5, 0.5)로 고정되어 있어야 하며,
/// 마지막 줄은 자식 수에 맞춰 중앙 정렬됩니다.
/// </summary>
[ExecuteAlways] // 에디터에서도 동작하게 만듭니다.
public class CustomGridLayout : MonoBehaviour
{
    public Vector2 cellSize = new Vector2(100, 100);
    public Vector2 spacing = new Vector2(10, 10);
    public int columnCount = 3;
    public bool isMiddleAlign = false;

    private RectTransform rectTransform;
    private bool needLayoutUpdate = false;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        RequestLayoutUpdate(); // OnEnable에서 큐
    }

    private void OnTransformChildrenChanged()
    {
        RequestLayoutUpdate();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdateLayout(); // 에디터에서는 즉시 실행
            return;
        }
#endif
        RequestLayoutUpdate(); // 실행 중에는 다음 프레임에 정렬
    }

    private void LateUpdate()
    {
        if (needLayoutUpdate)
        {
            UpdateLayout();
            needLayoutUpdate = false;
        }
    }

    public void RequestLayoutUpdate()
    {
        needLayoutUpdate = true;
    }

    public void UpdateLayout()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        int childCount = rectTransform.childCount;

        var activeChildren = new List<RectTransform>();
        for (int i = 0; i < childCount; i++)
        {
            var child = rectTransform.GetChild(i) as RectTransform;
            if (child != null && child.gameObject.activeSelf)
                activeChildren.Add(child);
        }

        int actualCount = activeChildren.Count;
        if (actualCount == 0) return;

        int rowCount = Mathf.CeilToInt((float)actualCount / columnCount);
        float totalHeight = (cellSize.y + spacing.y) * rowCount - spacing.y;
        Vector2 parentSize = rectTransform.rect.size;
        float baseOffsetY = isMiddleAlign ? -(parentSize.y - totalHeight) / 2f : 0f;

        for (int i = 0; i < actualCount; i++)
        {
            RectTransform child = activeChildren[i];
            int row = i / columnCount;
            int col = i % columnCount;

            bool isLastRow = row == rowCount - 1;
            int itemsInLastRow = actualCount % columnCount;
            if (itemsInLastRow == 0) itemsInLastRow = columnCount;

            int effectiveColCount = isLastRow ? itemsInLastRow : columnCount;
            float rowWidth = (cellSize.x + spacing.x) * effectiveColCount - spacing.x;
            float offsetX = (parentSize.x - rowWidth) / 2f;

            float x = (cellSize.x + spacing.x) * col + offsetX + cellSize.x * 0.5f;
            float y = -((cellSize.y + spacing.y) * row) + baseOffsetY - cellSize.y * 0.5f;

            child.anchorMin = new Vector2(0, 1);
            child.anchorMax = new Vector2(0, 1);
            child.pivot = new Vector2(0.5f, 0.5f);
            child.anchoredPosition = new Vector2(x, y);
            child.sizeDelta = cellSize;
        }
    }
}
