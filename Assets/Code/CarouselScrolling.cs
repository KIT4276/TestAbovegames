using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 [RequireComponent(typeof(ScrollRect))]
public class CarouselScrolling : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("Refs")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("Auto Scroll")]
    [SerializeField] private bool autoScroll = true;
    [SerializeField] private float autoScrollInterval = 5f;

    [Header("Snap")]
    [SerializeField] private float snapDuration = 0.25f;
    [SerializeField] private float snapEase = 8f; 
    [SerializeField] private float swipeThresholdNormalized = 0.08f;

    private RectTransform _viewport;
    private RectTransform _content;

    private int _pageCount;
    private int _currentPage;

    private bool _dragging;
    private Vector2 _dragStartLocalPos;
    private float _autoTimer;

    private Coroutine _snapRoutine;

    private void Reset() => 
        scrollRect = GetComponent<ScrollRect>();

    private void Awake()
    {
        if (!scrollRect) scrollRect = GetComponent<ScrollRect>();
        _viewport = scrollRect.viewport;
        _content = scrollRect.content;
    }

    private void OnEnable()
    {
        RebuildPages();
        SnapToPage(_currentPage, immediate: true);
        _autoTimer = 0f;
    }

    private void Update()
    {
        if (!autoScroll || _dragging || _pageCount <= 1) return;

        _autoTimer += Time.unscaledDeltaTime;
        if (_autoTimer >= autoScrollInterval)
        {
            _autoTimer = 0f;
            GoToPage(_currentPage + 1, wrap: true);
        }
    }

    public void RebuildPages()
    {
        if (!_content) return;
        _pageCount = _content.childCount;
        _currentPage = Mathf.Clamp(_currentPage, 0, Mathf.Max(0, _pageCount - 1));
    }

    public void GoToPage(int pageIndex, bool wrap)
    {
        if (_pageCount == 0) return;

        if (wrap)
        {
            pageIndex = (pageIndex % _pageCount + _pageCount) % _pageCount;
        }
        else
        {
            pageIndex = Mathf.Clamp(pageIndex, 0, _pageCount - 1);
        }

        _currentPage = pageIndex;
        SnapToPage(_currentPage, immediate: false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        _autoTimer = 0f;

        if (_snapRoutine != null)
        {
            StopCoroutine(_snapRoutine);
            _snapRoutine = null;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _viewport, eventData.position, eventData.pressEventCamera, out _dragStartLocalPos);
    }

    public void OnDrag(PointerEventData eventData) {}

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;

        Vector2 dragEndLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _viewport, eventData.position, eventData.pressEventCamera, out dragEndLocal);

        float dx = dragEndLocal.x - _dragStartLocalPos.x; 
        float threshold = _viewport.rect.width * swipeThresholdNormalized;

        int target = FindNearestPage();

        if (Mathf.Abs(dx) >= threshold)
        {
            target = dx > 0 ? target - 1 : target + 1;
        }

        target = Mathf.Clamp(target, 0, Mathf.Max(0, _pageCount - 1));
        _currentPage = target;

        SnapToPage(_currentPage, immediate: false);
    }

    private int FindNearestPage()
    {
        if (_pageCount <= 1) return 0;

        float viewportW = _viewport.rect.width;
        float pageW = viewportW; 

        float x = -_content.anchoredPosition.x;

        int nearest = Mathf.RoundToInt(x / Mathf.Max(1f, pageW));
        return Mathf.Clamp(nearest, 0, _pageCount - 1);
    }

    private void SnapToPage(int pageIndex, bool immediate)
    {
        if (_pageCount == 0) return;

        if (immediate)
        {
            scrollRect.StopMovement();
            scrollRect.horizontalNormalizedPosition = PageToNormalized(pageIndex);
            return;
        }

        if (_snapRoutine != null) StopCoroutine(_snapRoutine);
        _snapRoutine = StartCoroutine(SnapRoutine(PageToNormalized(pageIndex)));
    }

    private float PageToNormalized(int pageIndex)
    {
        if (_pageCount <= 1) return 0f;
        return Mathf.Clamp01(pageIndex / (float)(_pageCount - 1));
    }

    private IEnumerator SnapRoutine(float targetNormalized)
    {
        scrollRect.StopMovement();

        float t = 0f;
        float start = scrollRect.horizontalNormalizedPosition;

        while (t < snapDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - Mathf.Exp(-snapEase * (t / Mathf.Max(0.0001f, snapDuration)));
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, targetNormalized, k);
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = targetNormalized;
        _snapRoutine = null;
    }
}
