using UnityEngine;

public class Popups : MonoBehaviour
{
    [SerializeField] private PremiumPopup _premium;
    [SerializeField] private ImageViewPopup _imageView;

    private void Awake()
    {
        _premium.CloseClick += OnPremiumExitClick;
        _imageView.CloseClick += OnImageExitClick;
    }

    public void OpenPopup(bool isPremium, Sprite sprite)
    {
        if (isPremium)
        {
            _premium.gameObject.SetActive(true);
            _imageView.gameObject.SetActive(false);
        }
        else
        {
            _imageView.gameObject.SetActive(true);
            _premium.gameObject.SetActive(false);

            _imageView.Open(sprite);
        }
    }

    private void OnImageExitClick() =>
        _imageView.gameObject.SetActive(false);

    private void OnPremiumExitClick() =>
        _premium.gameObject.SetActive(false);

    private void OnDestroy()
    {
        if (_imageView != null) _imageView.CloseClick -= OnImageExitClick;
        if (_premium != null) _premium.CloseClick -= OnPremiumExitClick;
    }
}
