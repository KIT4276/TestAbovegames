using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    [SerializeField] private GalleryFilterUI _galleryUI;
    [SerializeField] private GalleryConfig _config;
    [SerializeField] private GalleryBuilderUI _galleryBuilderUI;
    [SerializeField] private RemoteSpriteService _remoteSpriteService;
    [SerializeField] private Popups _popups;

    private GalleryUIHandler _galleryUIHandler ;


    private void Awake()
    {
        _galleryUIHandler = new(_galleryUI, _config, _galleryBuilderUI);
        _galleryBuilderUI.Init(_remoteSpriteService, _config, _popups);
    }

    private void OnDestroy()
    {
        if (_galleryUIHandler != null)
        {
            _galleryUIHandler.Dispose();
            _galleryUIHandler = null;
        }
    }
}
