using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    [SerializeField] private GalleryUI _galleryUI;
    [SerializeField] private GalleryConfig _config;
    [SerializeField] private GalleryBuilderUI _galleryBuilderUI;
    [SerializeField] private RemoteSpriteService _remoteSpriteService;

    public GalleryFilterBuilder GalleryFilterBuilder { get; private set; }
    public GalleryConfig Config => _config; 
    public GalleryUIHandler GalleryUIHandler { get; private set; }


    private void Awake()
    {
        GalleryFilterBuilder = new();
        GalleryUIHandler = new(_galleryUI, _config, _galleryBuilderUI);

        _galleryBuilderUI.Init(_remoteSpriteService, _config);
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
