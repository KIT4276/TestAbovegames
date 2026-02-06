using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryLazyLoader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private RectTransform _content;
    [SerializeField] private GridLayoutGroup _grid;

    [Header("Tuning")]
    [SerializeField] private int _bufferRows = 2;

    private IRemoteSpriteService _remote;
    private GalleryConfig _config;

    private GalleryElement[] _elements;
    private List<int> _ids = new();

    // чтобы не дергать загрузку повторно
    private readonly HashSet<string> _requested = new();

    public void Init(IRemoteSpriteService remote, GalleryConfig config, GalleryElement[] elements)
    {
        _remote = remote;
        _config = config;
        _elements = elements;

        // Важно для корректного anchoredPosition.y при скролле
        SetTopPivot(_content);

        _scrollRect.onValueChanged.AddListener(_ => UpdateVisible());
    }

    private void OnDestroy()
    {
        if (_scrollRect != null)
            _scrollRect.onValueChanged.RemoveAllListeners();
    }

    public void SetIds(List<int> ids)
    {
        _ids = ids ?? new List<int>();
        _requested.Clear();

        // включаем нужное количество карточек, остальное скрываем (только на фильтре)
        for (int i = 0; i < _elements.Length; i++)
        {
            if (i < _ids.Count)
            {
                int id = _ids[i];
                _elements[i].Activate(true);
                _elements[i].Bind(id, (id % 4) == 0); // premium каждый 4-й
            }
            else
            {
                _elements[i].Activate(false);
            }
        }

        Canvas.ForceUpdateCanvases();
        UpdateVisible();
    }

    private void UpdateVisible()
    {
        if (_ids == null || _ids.Count == 0) return;

        int columns = GetColumns();
        float cellH = _grid.cellSize.y + _grid.spacing.y;

        // anchoredPosition.y растёт при скролле вниз (при pivot.y = 1)
        float y = Mathf.Max(0f, _content.anchoredPosition.y);

        float top = y - _grid.padding.top;
        float bottom = y + _viewport.rect.height - _grid.padding.top;

        int firstRow = Mathf.FloorToInt(top / Mathf.Max(1f, cellH)) - _bufferRows;
        int lastRow = Mathf.FloorToInt(bottom / Mathf.Max(1f, cellH)) + _bufferRows;

        int totalRows = Mathf.CeilToInt(_ids.Count / (float)columns);
        firstRow = Mathf.Clamp(firstRow, 0, Mathf.Max(0, totalRows - 1));
        lastRow = Mathf.Clamp(lastRow, 0, Mathf.Max(0, totalRows - 1));

        int firstIndex = firstRow * columns;
        int lastIndex = Mathf.Min(_ids.Count - 1, (lastRow * columns) + (columns - 1));

        for (int i = firstIndex; i <= lastIndex; i++)
        {
            int id = _ids[i];
            string url = _config.GetUrl(id);

            if (_requested.Contains(url)) continue;
            _requested.Add(url);

            // элемент соответствует индексу, т.к. мы не виртуализируем
            var element = _elements[i];

            if (_remote.TryGetCached(url, out var cached) && cached != null)
            {
                element.ApplySpriteIfStillBound(id, cached);
                continue;
            }

            _remote.Load(
                url,
                onSuccess: sprite => element.ApplySpriteIfStillBound(id, sprite),
                onError: msg => Debug.LogError(msg)
            );
        }
    }

    private int GetColumns()
    {
        // Если у GridLayoutGroup стоит FixedColumnCount — берём его
        if (_grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            return Mathf.Max(1, _grid.constraintCount);

        // Иначе считаем по ширине (на всякий)
        float w = _viewport.rect.width - _grid.padding.left - _grid.padding.right;
        float step = _grid.cellSize.x + _grid.spacing.x;
        return Mathf.Max(1, Mathf.FloorToInt((w + _grid.spacing.x) / Mathf.Max(1f, step)));
    }

    private static void SetTopPivot(RectTransform rt)
    {
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
    }
}
