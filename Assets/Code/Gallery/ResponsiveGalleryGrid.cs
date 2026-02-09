using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGalleryGrid : MonoBehaviour
{
    [SerializeField] private DeviceMode _mode = DeviceMode.Auto;

    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private RectTransform _widthSource; 

    [Header("Columns")]
    [SerializeField] private int _phoneColumns = 2;
    [SerializeField] private int _tabletColumns = 3;

    [Header("Tablet detection")]
    [SerializeField] private float tabletMinDiagonalInches = 7.0f;

    [Header("Cell")]
    [SerializeField] private bool _squareCells = true;
    [SerializeField] private float _fixedCellHeight = 640f;
    public enum DeviceMode { Auto, ForcePhone, ForceTablet }

    private int _lastCols = -1;
    private float _lastW = -999f;

    private void Reset()
    {
        _grid = GetComponent<GridLayoutGroup>();
        _widthSource = GetComponent<RectTransform>();
    }

    private void OnEnable() => Apply();
    private void OnRectTransformDimensionsChange() => Apply();

    private void Apply()
    {
        if (!_grid || !_widthSource) return;

        int cols = IsTablet() ? _tabletColumns : _phoneColumns;

        float w = _widthSource.rect.width;
        if (cols == _lastCols && Mathf.Abs(w - _lastW) < 0.5f) return;
        _lastCols = cols;
        _lastW = w;

        _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _grid.constraintCount = cols;

        float totalPadding = _grid.padding.left + _grid.padding.right;
        float totalSpacing = _grid.spacing.x * (cols - 1);
        float cellW = (w - totalPadding - totalSpacing) / cols;

        float cellH = _squareCells ? cellW : _fixedCellHeight;
        _grid.cellSize = new Vector2(cellW, cellH);
    }

    private bool IsTablet()
    {
        if (_mode == DeviceMode.ForceTablet) return true;
        if (_mode == DeviceMode.ForcePhone) return false;

        float dpi = Screen.dpi;

        if (dpi <= 0f) return false;

        float wIn = Screen.width / dpi;
        float hIn = Screen.height / dpi;
        float diag = Mathf.Sqrt(wIn * wIn + hIn * hIn);

        return diag >= tabletMinDiagonalInches;
    }
}
