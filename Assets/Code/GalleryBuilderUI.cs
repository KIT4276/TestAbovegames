using UnityEngine;

public class GalleryBuilderUI : MonoBehaviour
{
    [SerializeField] private GalleryElement[] _galleryElements;

    private IRemoteSpriteService _remoteSpriteService;
    private GalleryConfig _config;

    private void Awake()
    {
        foreach (var element in _galleryElements)
            element.Activate(false);
    }

    public void Init(IRemoteSpriteService remoteSpriteService, GalleryConfig config)
    {
        _remoteSpriteService = remoteSpriteService;
        _config = config;


        // TODO спавнит нужное количество GalleryElement
    }

    public void OnFilterApplied(GalleryFilter filter)
    {
        var ids = GalleryFilterBuilder.BuildIds(_config.MinImages, _config.TotalImages, filter);

        int max;

        if(ids.Count> _galleryElements.Length) max = _galleryElements.Length;
        else max = ids.Count;

        for (int i = 0; i< max; i ++)
        {
            if(i >= _galleryElements.Length) return;

            _remoteSpriteService.Load(_config.BaseUrl + ids[i] + ".jpg",
                onSuccess: _galleryElements[i].SetImage,
                onError: OnLoadError);
        }
    }

    private void OnLoadError(string message) =>
        Debug.LogError(message);
}
