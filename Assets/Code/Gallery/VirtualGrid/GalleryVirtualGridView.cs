using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryVirtualGridView : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private RectTransform _content;
    [SerializeField] private GalleryElement _cardPrefab;

    public RectTransform Viewport => _viewport;
    public RectTransform Content => _content;

    public event Action Scrolled;

    private void Awake()
    {
        _scrollRect.onValueChanged.AddListener(_ => Scrolled?.Invoke());
    }

    private void OnDestroy()
    {
        if (_scrollRect != null)
            _scrollRect.onValueChanged.RemoveAllListeners();
    }

    public void SetContentHeight(float h)
    {
        var s = _content.sizeDelta;
        s.y = h;
        _content.sizeDelta = s;
    }

    public GalleryElement CreateItem()
    {
        return Instantiate(_cardPrefab, _content, false);
    }
}
