using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GalleryFilterUI : MonoBehaviour
{
    [SerializeField] private Toggle _allToggle;
    [SerializeField] private Toggle _oddToggle;
    [SerializeField] private Toggle _evenToggle;
    [Space]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _content;

    public event Action<GalleryFilter> FilterApplied;

    private GalleryFilter _currentFilter = GalleryFilter.Odd;

    private void Awake()
    {
        _allToggle.onValueChanged.AddListener(OnToggleChanged);
        _oddToggle.onValueChanged.AddListener(OnToggleChanged);
        _evenToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void Start()
    {
        _oddToggle.isOn = true;
        ApplyFilter(GetCurrentFilter(), force: true);
    }

    public GalleryFilter GetCurrentFilter()
    {
        if (_allToggle != null && _allToggle.isOn) return GalleryFilter.All;
        if (_oddToggle != null && _oddToggle.isOn) return GalleryFilter.Odd;
        if (_evenToggle != null && _evenToggle.isOn) return GalleryFilter.Even;

        return GalleryFilter.Non;
    }

    private void OnToggleChanged(bool arg0) =>
        ApplyFilter(GetCurrentFilter());

    public void ResetVerticalPosition()
    {
        if (_scrollRect == null || _content == null) return;

        _scrollRect.StopMovement();
        _scrollRect.velocity = Vector2.zero;

        StartCoroutine(ResetVerticalPositionRoutine());
    }

    private void ApplyFilter(GalleryFilter filter,bool force = false)
    {
        if(!force && _currentFilter == filter) return;

        _currentFilter = filter;
        FilterApplied?.Invoke(filter);
    }

    private IEnumerator ResetVerticalPositionRoutine()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        Canvas.ForceUpdateCanvases();

        _scrollRect.verticalNormalizedPosition = 1f;
        _scrollRect.StopMovement();
        _scrollRect.velocity = Vector2.zero;
    }

    private void OnDisable()
    {
        _allToggle.onValueChanged.RemoveListener(OnToggleChanged);
        _oddToggle.onValueChanged.RemoveListener(OnToggleChanged);
        _evenToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}
