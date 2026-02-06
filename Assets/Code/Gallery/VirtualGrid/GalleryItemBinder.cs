using UnityEngine;

public class GalleryItemBinder//загрузка + премиум
{
    private readonly IRemoteSpriteService _remote;
    private readonly GalleryConfig _config;

    public GalleryItemBinder(IRemoteSpriteService remote, GalleryConfig config)
    {
        _remote = remote;
        _config = config;
    }

    public void Bind(GalleryElement el, int id)
    {
        bool premium = (id % 4) == 0;
        el.Bind(id, premium);

        string url = _config.GetUrl(id);

        if (_remote.TryGetCached(url, out var cached) && cached != null)
        {
            el.ApplySpriteIfStillBound(id, cached);
            return;
        }

        _remote.Load(
            url,
            onSuccess: sprite => el.ApplySpriteIfStillBound(id, sprite),
            onError: msg => Debug.LogError(msg)
        );
    }
}


