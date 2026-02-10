using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteSpriteService : MonoBehaviour, IRemoteSpriteService
{
    [Header("Network")]
    [SerializeField] private int _maxParallelRequests = 1;
    [SerializeField] private int _timeoutSeconds = 60;
    [SerializeField] private float _retryCooldownSeconds = 10f;

    [Header("Sprite")]
    [SerializeField] private float _pixelsPerUnit = 100f;

    private readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, List<Action<Sprite>>> _pending = new Dictionary<string, List<Action<Sprite>>>();
    private readonly Queue<string> _queue = new Queue<string>();
    private readonly Dictionary<string, float> _cooldownUntil = new();

    private int _inFlight;

    public void Load(string url, Action<Sprite> onSuccess, Action<string> onError = null)
    {
        if (string.IsNullOrEmpty(url))
        {
            if (onError != null) onError("URL is null or empty.");
            return;
        }

        if (_cooldownUntil.TryGetValue(url, out var until) && Time.unscaledTime < until)
        {
            onError?.Invoke($"Cooldown: {url}");
            return;
        }

        Sprite cached;
        if (_cache.TryGetValue(url, out cached) && cached != null)
        {
            if (onSuccess != null) onSuccess(cached);
            return;
        }

        List<Action<Sprite>> list;
        if (_pending.TryGetValue(url, out list))
        {
            if (onSuccess != null) list.Add(onSuccess);
            return;
        }

        list = new List<Action<Sprite>>();
        if (onSuccess != null) list.Add(onSuccess);
        _pending[url] = list;

        _queue.Enqueue(url);
        TryStartNext(onError);
    }

    public bool TryGetCached(string url, out Sprite sprite)
    {
        return _cache.TryGetValue(url, out sprite) && sprite != null;
    }

    public void ClearCache(bool destroySprites)
    {
        if (destroySprites)
        {
            foreach (var kv in _cache)
            {
                Sprite s = kv.Value;
                if (s != null)
                {
                    Texture2D tex = s.texture;
                    Destroy(s);
                    if (tex != null) Destroy(tex);
                }
            }
        }

        _cache.Clear();
        _pending.Clear();
        _queue.Clear();
        _inFlight = 0;
    }

    private void TryStartNext(Action<string> onError)
    {
        while (_inFlight < _maxParallelRequests && _queue.Count > 0)
        {
            string url = _queue.Dequeue();
            _inFlight++;
            StartCoroutine(LoadRoutine(url, onError));
        }
    }

    private IEnumerator LoadRoutine(string url, Action<string> onError)
    {
        try
        {

            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(url, true)) // true = nonReadable (экономия памяти)
            {
                req.timeout = _timeoutSeconds;

                Debug.Log($"[Net] START {url} timeout={req.timeout}");

                yield return req.SendWebRequest();

                Debug.Log($"[Net] END {url} result={req.result} code={req.responseCode} err={req.error} bytes={req.downloadedBytes}");

                if (req.result != UnityWebRequest.Result.Success)
                {
                    Fail(url, req.error, onError);
                    yield break;
                }

                Texture2D tex = DownloadHandlerTexture.GetContent(req);
                if (tex == null)
                {
                    Fail(url, "Texture is null.", onError);
                    yield break;
                }

                Sprite sprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    _pixelsPerUnit
                );

                sprite.name = "RemoteSprite_" + url;

                _cache[url] = sprite;

                List<Action<Sprite>> callbacks;
                if (_pending.TryGetValue(url, out callbacks))
                {
                    _pending.Remove(url);
                    for (int i = 0; i < callbacks.Count; i++)
                    {
                        Action<Sprite> cb = callbacks[i];
                        if (cb != null) cb(sprite);
                    }
                }
            }
        }
        finally
        {
            _inFlight--;
            TryStartNext(onError);
        }
    }

    private void Fail(string url, string error, Action<string> onError)
    {

        if (onError != null) onError("Failed to load sprite: " + url + " | " + error);

        _pending.Remove(url);
        _cooldownUntil[url] = Time.unscaledTime + _retryCooldownSeconds;

        //_inFlight--;
        // TryStartNext(onError);
    }
}

public interface IRemoteSpriteService
{
    void Load(string url, Action<Sprite> onSuccess, Action<string> onError = null);
    bool TryGetCached(string url, out Sprite sprite);
    void ClearCache(bool destroySprites);
}