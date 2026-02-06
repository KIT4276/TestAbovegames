using System;
using UnityEngine;
using UnityEngine.UI;

public class GalleryElement : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _card;
    [SerializeField]  private GameObject _premiumBadge;

    public int BoundId { get; private set; }

    public void SetImage(Sprite sprite)
    {
        //Activate(true);
        _image.sprite = sprite;
    }

    //public void Activate(bool isActive) => 
    //    _card.SetActive(isActive);

    public void ApplySpriteIfStillBound(int id, Sprite sprite)
    {
        if (BoundId != id) return;
        if (_image == null) return;

        _image.sprite = sprite;
    }

    public void Bind(int id, bool isPremium)
    {
        BoundId = id;

        if (_premiumBadge != null)
            _premiumBadge.SetActive(isPremium);

        if (_image != null)
            _image.sprite = null;
    }
}
