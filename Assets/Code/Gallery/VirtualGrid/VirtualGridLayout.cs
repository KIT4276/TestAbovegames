using UnityEngine;

public class VirtualGridLayout//чистые расчёты
{
    public int Columns { get; private set; }
    public float CellW { get; private set; }
    public float CellH { get; private set; }
    public float RowH { get; private set; }

    private readonly Vector2 _spacing;
    private readonly RectOffset _padding;

    public VirtualGridLayout(Vector2 spacing, RectOffset padding, float cellHeight)
    {
        _spacing = spacing;
        _padding = padding;
        CellH = cellHeight;
    }

    public float Rebuild(RectTransform viewport, int itemCount, int columns)
    {
        Columns = Mathf.Max(1, columns);

        float vw = viewport.rect.width;
        CellW = (vw - _padding.left - _padding.right - _spacing.x * (Columns - 1)) / Columns;
        RowH = CellH + _spacing.y;

        int rows = Mathf.CeilToInt(itemCount / (float)Columns);

        float h = _padding.top + _padding.bottom;
        if (rows > 0)
            h += rows * CellH + Mathf.Max(0, rows - 1) * _spacing.y;

        return h;
    }

    public void GetItemRect(int dataIndex, out Vector2 pos, out Vector2 size)
    {
        int row = dataIndex / Columns;
        int col = dataIndex % Columns;

        float x = _padding.left + col * (CellW + _spacing.x);
        float y = -_padding.top - row * (CellH + _spacing.y);

        pos = new Vector2(x, y);
        size = new Vector2(CellW, CellH);
    }
}
