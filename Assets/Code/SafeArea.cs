using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    [Tooltip("Apply safe area on Awake and when resolution/orientation changes.")]
    [SerializeField] private bool _updateContinuously = true;
    [SerializeField] private RectTransform _rt;

    private Rect _lastSafeArea;
    private Vector2Int _lastScreenSize;
    private ScreenOrientation _lastOrientation;

    private void Awake()
    {
        if (_rt == null)
            _rt = GetComponent<RectTransform>();

        ApplySafeArea();
    }

    private void OnEnable() =>
        ApplySafeArea();

    private void Update()
    {
        if (!_updateContinuously) return;

        if (_lastSafeArea != Screen.safeArea ||
            _lastScreenSize.x != Screen.width ||
            _lastScreenSize.y != Screen.height ||
            _lastOrientation != Screen.orientation)
        {
            ApplySafeArea();
        }
    }

    public void ApplySafeArea()
    {
        Rect safe = Screen.safeArea;

        _lastSafeArea = safe;
        _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        _lastOrientation = Screen.orientation;

        Vector2 anchorMin = new Vector2(
            safe.xMin / Screen.width,
            safe.yMin / Screen.height
        );

        Vector2 anchorMax = new Vector2(
            safe.xMax / Screen.width,
            safe.yMax / Screen.height
        );

        _rt.anchorMin = anchorMin;
        _rt.anchorMax = anchorMax;
        _rt.offsetMin = Vector2.zero;
        _rt.offsetMax = Vector2.zero;
    }
}
