using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GalleryElement : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _premiumBadge;

    public int BoundId { get; private set; }

    private bool _isPremium;
    private Popups _popups;

    public void Init(Popups popups) =>
        _popups = popups;

    public void ApplySpriteIfStillBound(int id, Sprite sprite)
    {
        if (BoundId != id) return;
        if (_image == null) return;

        _image.sprite = sprite;
    }

    public void Bind(int id, bool isPremium)
    {
        _isPremium = isPremium;

        BoundId = id;

        if (_premiumBadge != null)
            _premiumBadge.SetActive(isPremium);

        if (_image != null)
            _image.sprite = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_popups == null || _image.sprite == null) return;

        _popups.OpenPopup(_isPremium, _image.sprite);
    }
}
