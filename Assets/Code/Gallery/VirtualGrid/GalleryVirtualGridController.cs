using System.Collections.Generic;
using UnityEngine;

public class GalleryVirtualGridController
{
    private readonly GalleryVirtualGridView _view;
    private readonly VirtualGridLayout _layout;
    private readonly GalleryItemBinder _binder;

    private readonly List<int> _ids = new();
    private readonly Queue<GalleryElement> _free = new();
    private readonly Dictionary<int, GalleryElement> _visible = new();

    private readonly int _bufferRows;

    public GalleryVirtualGridController(
        GalleryVirtualGridView view,
        VirtualGridLayout layout,
        GalleryItemBinder binder,
        int bufferRows)
    {
        _view = view;
        _layout = layout;
        _binder = binder;
        _bufferRows = bufferRows;

        SetTopPivot(_view.Content);
        _view.Scrolled += UpdateVisible;
    }

    public void SetData(List<int> ids)
    {
        _ids.Clear();
        if (ids != null) _ids.AddRange(ids);

        ClearVisible();

        int columns = IsTablet() ? 3 : 2;
        float contentH = _layout.Rebuild(_view.Viewport, _ids.Count, columns);
        _view.SetContentHeight(contentH);

        EnsurePool();
        UpdateVisible();
    }

    private void EnsurePool()
    {
        int columns = _layout.Columns;
        int visibleRows = Mathf.CeilToInt(_view.Viewport.rect.height / Mathf.Max(1f, _layout.RowH)) + 1;
        int need = (visibleRows + _bufferRows * 2) * columns;

        while (_free.Count + _visible.Count < need)
        {
            var el = _view.CreateItem();
            el.Activate(false);
            _free.Enqueue(el);
        }
    }

    private void UpdateVisible()
    {
        if (_ids.Count == 0) return;

        float y = _view.Content.anchoredPosition.y;
        float top = Mathf.Max(0f, y);
        float bottom = y + _view.Viewport.rect.height;

        int firstRow = Mathf.FloorToInt(top / Mathf.Max(1f, _layout.RowH)) - _bufferRows;
        int lastRow = Mathf.FloorToInt(bottom / Mathf.Max(1f, _layout.RowH)) + _bufferRows;

        int totalRows = Mathf.CeilToInt(_ids.Count / (float)_layout.Columns);
        firstRow = Mathf.Clamp(firstRow, 0, Mathf.Max(0, totalRows - 1));
        lastRow = Mathf.Clamp(lastRow, 0, Mathf.Max(0, totalRows - 1));

        int firstIndex = firstRow * _layout.Columns;
        int lastIndex = Mathf.Min(_ids.Count - 1, lastRow * _layout.Columns + (_layout.Columns - 1));

        // убрать лишние
        var toRemove = ListPool<int>.Get();
        foreach (var kv in _visible)
            if (kv.Key < firstIndex || kv.Key > lastIndex)
                toRemove.Add(kv.Key);

        for (int i = 0; i < toRemove.Count; i++)
        {
            int idx = toRemove[i];
            var el = _visible[idx];
            _visible.Remove(idx);
            Release(el);
        }
        ListPool<int>.Release(toRemove);

        // добавить нужные
        for (int idx = firstIndex; idx <= lastIndex; idx++)
        {
            if (_visible.ContainsKey(idx)) continue;
            if (_free.Count == 0) break;

            var el = _free.Dequeue();
            _visible[idx] = el;

            _layout.GetItemRect(idx, out var pos, out var size);

            //el.SetRectTransformParams(new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), pos, size);

            el.Activate(true);

            int id = _ids[idx];
            _binder.Bind(el, id);
        }
    }

    private void Release(GalleryElement el)
    {
        el.Activate(false);
        _free.Enqueue(el);
    }

    private void ClearVisible()
    {
        foreach (var kv in _visible)
            Release(kv.Value);
        _visible.Clear();
    }

    private static void SetTopPivot(RectTransform rt)
    {
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
    }

    private static bool IsTablet()
    {
        float dpi = Screen.dpi;
        if (dpi <= 0f) dpi = 160f;
        float shortest = Mathf.Min(Screen.width, Screen.height);
        float dp = shortest * 160f / dpi;
        return dp >= 600f;
    }
}

// простая пул-утилита, чтобы не аллоцировать списки
static class ListPool<T>
{
    static readonly Stack<List<T>> _pool = new();
    public static List<T> Get() => _pool.Count > 0 ? _pool.Pop() : new List<T>();
    public static void Release(List<T> list) { list.Clear(); _pool.Push(list); }
}


