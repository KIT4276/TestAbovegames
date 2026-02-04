using System;
using UnityEngine;
using UnityEngine.UI;

public class GalleryUI : MonoBehaviour
{
    [SerializeField] private Toggle _allToggle;
    [SerializeField] private Toggle _oddToggle;
    [SerializeField] private Toggle _evenToggle;

    public event Action<GalleryFilter> FilterApplied;

    private GalleryFilter _currentFilter;

    private void Awake()
    {
        OnToggleChanged(true);

        _allToggle.onValueChanged.AddListener(OnToggleChanged);
        _oddToggle.onValueChanged.AddListener(OnToggleChanged);
        _evenToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool arg0) =>
        ApplyFilter(GetCurrentFilter());

    public GalleryFilter GetCurrentFilter()
    {
        if (_allToggle != null && _allToggle.isOn) return GalleryFilter.All;
        if (_oddToggle != null && _oddToggle.isOn) return GalleryFilter.Odd;
        if (_evenToggle != null && _evenToggle.isOn) return GalleryFilter.Even;

        return GalleryFilter.Non;
    }

    private void ApplyFilter(GalleryFilter filter)
    {
        if(_currentFilter == filter) return;
        _currentFilter = filter;
        FilterApplied?.Invoke(filter);
    }

    private void OnDisable()
    {
        _allToggle.onValueChanged.RemoveListener(OnToggleChanged);
        _oddToggle.onValueChanged.RemoveListener(OnToggleChanged);
        _evenToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}

public class GalleryBuilder : MonoBehaviour
{
    [SerializeField] private GalleryElement[] _galleryElements;

    public void OnFilterApplied(GalleryFilter filter)
    {
        //спавнит нужное количество, заполняет элементы
    }

}
