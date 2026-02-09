using UnityEngine;
using UnityEngine.UI;

public class ImageViewPopup : BasePopup
{
    [SerializeField] private Image _popupImage;

    public void Open(Sprite sprite) => 
        _popupImage.sprite = sprite;

    protected override void Awake() => 
        base.Awake();

    protected override void OnDestroy() => 
        base.OnDestroy();
}