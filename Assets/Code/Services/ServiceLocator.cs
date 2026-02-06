using UnityEngine;
using UnityEngine.tvOS;

public class ServiceLocator : MonoBehaviour
{
    [SerializeField] private GalleryFilterUI _galleryUI;
    [SerializeField] private GalleryConfig _config;
    //[SerializeField] private RemoteSpriteService _remoteSpriteService;
    [SerializeField] private RemoteSpriteService _remote;
    [SerializeField] private GalleryVirtualGridView _gridView;

    private GalleryVirtualGridController _gridController;

    private GalleryUIHandler _galleryUIHandler ;


    private void Awake()
    {
        var layout = new VirtualGridLayout(
       spacing: new Vector2(24, 24),
       padding: new RectOffset(24, 24, 24, 24),
        cellHeight: 360f);

        var binder = new GalleryItemBinder(_remote, _config);

        _gridController = new GalleryVirtualGridController(_gridView, layout, binder, bufferRows: 2);

        _galleryUIHandler = new(_galleryUI, _config, _gridController);
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
