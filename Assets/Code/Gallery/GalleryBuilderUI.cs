using System.Collections.Generic;
using UnityEngine;

public class GalleryBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private RectTransform _content;
    [SerializeField] private GalleryLazyLoader _lazyLoader;

    private GalleryElement[] _galleryElements;

    private IRemoteSpriteService _remoteSpriteService;
    private GalleryConfig _config;

    public void Init(IRemoteSpriteService remoteSpriteService, GalleryConfig config, Popups popups)
    {
        _remoteSpriteService = remoteSpriteService;
        _config = config;

        List<GalleryElement> list = new();

        for (int i = 0; i < _config.TotalImages; i++)
        {
            var go = Instantiate(_cardPrefab, _content, false);
            var element = go.GetComponent<GalleryElement>();

            if (element == null)
            {
                Debug.LogError("Card prefab has no GalleryElement component.");
                Destroy(go);
                continue;
            }

            go.SetActive(false);
            list.Add(element);
        }
        _galleryElements = list.ToArray();

        foreach (var element in _galleryElements)
            element.Init(popups);

        _lazyLoader.Init(_remoteSpriteService, _config, _galleryElements);
    }

    public void OnFilterApplied(GalleryFilter filter)
    {
        var ids = GalleryFilterBuilder.BuildIds(_config.MinImages, _config.TotalImages, filter);
        _lazyLoader.SetIds(ids);
    }

    private void OnLoadError(string message) =>
        Debug.LogError(message);
}
