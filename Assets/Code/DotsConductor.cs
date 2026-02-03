using UnityEngine;

public class DotsConductor : MonoBehaviour
{
    [SerializeField] private Dot[] _dots;
    [SerializeField] private CarouselScrolling _carouselScrolling;

    private void Awake() => 
        _carouselScrolling.PageChanged += OnPageChanged;

    private void OnPageChanged(int page)
    {
        if (page < 0 || page >= _dots.Length)
        {
            Debug.LogWarning("The number of slides does not match!");
            return;
        }

        var activeDot = _dots[page];
        activeDot.ChangeActivity(true);
        foreach (var dot in _dots)
        {
            if (dot != activeDot)
                dot.ChangeActivity(false);
        }
    }

    private void OnDestroy() => 
        _carouselScrolling.PageChanged -= OnPageChanged;
}
