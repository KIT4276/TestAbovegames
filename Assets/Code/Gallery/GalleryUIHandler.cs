using System;
using System.Collections.Generic;

public class GalleryUIHandler : IDisposable
{
    private readonly GalleryFilterUI _galleryUI;
    private readonly GalleryConfig _galleryConfig;
    private readonly GalleryVirtualGridController _gridController;

    private List<int> _currentIds;

    public GalleryUIHandler(GalleryFilterUI galleryUI, GalleryConfig galleryConfig, 
       GalleryVirtualGridController gridController)
    {
        _galleryUI = galleryUI;
        _galleryConfig = galleryConfig;
        _gridController = gridController;

        _galleryUI.FilterApplied += OnFilterApplied;
    }

    public void Dispose() =>
        _galleryUI.FilterApplied -= OnFilterApplied;

    private void OnFilterApplied(GalleryFilter filter)
    {
        _currentIds = GalleryFilterBuilder.BuildIds(_galleryConfig.MinImages, _galleryConfig.TotalImages, filter);
        _galleryUI.ResetVerticalPosition();

        _gridController.SetData(_currentIds);
    }
}
