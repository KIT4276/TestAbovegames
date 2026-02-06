using UnityEngine;
using UnityEngine.UI;

public class GalleryElement : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _card;
   // [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GameObject _premiumBadge;

    public int BoundId { get; private set; }

    public void SetImage(Sprite sprite)
    {
        Activate(true);
        _image.sprite = sprite;
    }

    public void Activate(bool isActive) => 
        _card.SetActive(isActive);

    

    /// <summary>
    /// Привязать элемент к конкретному id (данным) и выставить визуальные состояния.
    /// Вызывается каждый раз, когда элемент переиспользуется пулом.
    /// </summary>
    public void Bind(int id, bool isPremium)
    {
        BoundId = id;

        if (_premiumBadge != null)
            _premiumBadge.SetActive(isPremium);

        // Сбросить старую картинку, чтобы не мигала при переиспользовании
        if (_image != null)
            _image.sprite = null;

        // Если есть отдельный placeholder-спрайт — можно поставить его здесь вместо null
        // _image.sprite = _placeholderSprite;
    }

    /// <summary>
    /// Применить спрайт только если элемент всё ещё привязан к этому id.
    /// Защита от "поздних" ответов сети, когда элемент уже переиспользован под другой id.
    /// </summary>
    public void ApplySpriteIfStillBound(int id, Sprite sprite)
    {
        if (BoundId != id) return;
        if (_image == null) return;

        _image.sprite = sprite;
    }
}
