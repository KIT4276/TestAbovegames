using System;
using System.Collections.Generic;

public class GalleryUIHandler : IDisposable
{
    private readonly GalleryUI _galleryUI;
    private readonly GalleryConfig _galleryConfig;
    private readonly CarouselScrolling _carouselScrolling;

    private List<int> _currentIds;

    public GalleryUIHandler(GalleryUI galleryUI, GalleryConfig galleryConfig, CarouselScrolling carouselScrolling)
    {
        _galleryUI = galleryUI;
        _galleryConfig = galleryConfig;
        _carouselScrolling = carouselScrolling;

        _galleryUI.FilterApplied += OnFilterApplied;
    }

    private void OnFilterApplied(GalleryFilter filter)
    {
        _currentIds = GalleryFilterBuilder.BuildIds(_galleryConfig.MinImages, _galleryConfig.TotalImages, filter);
        _carouselScrolling.ResetVerticalPosition();
        //TODO:
        //_virtualGrid.SetData(_currentIds, _config.baseUrl); 
        //GalleryBuilder.OnFilterApplied(filter); вызывается после _virtualGrid.SetData млм прям вот там же
    }

    public void Dispose() =>
        _galleryUI.FilterApplied -= OnFilterApplied;
}
