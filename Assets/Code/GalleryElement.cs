using UnityEngine;
using UnityEngine.UI;

public class GalleryElement : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _card;

    public void SetImage(Sprite sprite)
    {
        Activate(true);
        _image.sprite = sprite;
    }

    public void Activate(bool isActive) => 
        _card.SetActive(isActive);
}
