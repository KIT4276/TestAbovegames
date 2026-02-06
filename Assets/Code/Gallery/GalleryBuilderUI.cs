using System.Collections.Generic;
using UnityEngine;

public class GalleryBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private RectTransform _content;

    private GalleryElement[] _galleryElements;

    private IRemoteSpriteService _remoteSpriteService;
    private GalleryConfig _config;

    public void Init(IRemoteSpriteService remoteSpriteService, GalleryConfig config)
    {
        _remoteSpriteService = remoteSpriteService;
        _config = config;

        List<GalleryElement> elements = new();

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

            element.Activate(false);
            elements.Add(element);
        }

        _galleryElements = elements.ToArray();
    }

    public void OnFilterApplied(GalleryFilter filter)
    {
        var ids = GalleryFilterBuilder.BuildIds(_config.MinImages, _config.TotalImages, filter);

        for (int i = ids.Count; i < _galleryElements.Length; i++)
            _galleryElements[i].Activate(false);

        for (int i = 0; i < ids.Count && i < _galleryElements.Length; i++)
        {
            int id = ids[i];
            _remoteSpriteService.Load(
                _config.GetUrl(id),
                onSuccess: _galleryElements[i].SetImage,
                onError: OnLoadError
            );
        }
    }

    private void OnLoadError(string message) =>
        Debug.LogError(message);
}
