using System;
using System.Collections.Generic;

public class GalleryUIHandler : IDisposable
{
    private readonly GalleryUI _galleryUI;
    private readonly GalleryConfig _galleryConfig;
    private readonly GalleryBuilderUI _galleryBuilderUI;

    private List<int> _currentIds;

    public GalleryUIHandler(GalleryUI galleryUI, GalleryConfig galleryConfig, GalleryBuilderUI galleryBuilderUI)
    {
        _galleryUI = galleryUI;
        _galleryConfig = galleryConfig;
        _galleryBuilderUI = galleryBuilderUI;

        _galleryUI.FilterApplied += OnFilterApplied;
    }

    public void Dispose() =>
        _galleryUI.FilterApplied -= OnFilterApplied;

    private void OnFilterApplied(GalleryFilter filter)
    {
        _currentIds = GalleryFilterBuilder.BuildIds(_galleryConfig.MinImages, _galleryConfig.TotalImages, filter);
        _galleryUI.ResetVerticalPosition();
        //TODO:
        //_virtualGrid.SetData(_currentIds, _config.baseUrl); 
        _galleryBuilderUI.OnFilterApplied(filter);// вызывается после _virtualGrid.SetData или прям вот там же
    }
}
