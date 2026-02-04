using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    [SerializeField] private GalleryUI _galleryUI;
    [SerializeField] private GalleryConfig _config;
    [SerializeField] private CarouselScrolling _carouselScrolling;

    public GalleryFilterBuilder GalleryFilterBuilder { get; private set; }
    public GalleryConfig Config => _config; 
    public CarouselScrolling CarouselScrolling => _carouselScrolling;
    public GalleryUIHandler GalleryUIHandler { get; private set; }

    private void Awake()
    {
        GalleryFilterBuilder = new();
        GalleryUIHandler = new(_galleryUI, _config, _carouselScrolling);
    }

    private void OnDestroy()
    {
        if (GalleryUIHandler != null)
        {
            GalleryUIHandler.Dispose();
            GalleryUIHandler = null;
        }
    }
}
