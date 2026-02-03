using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _activeImage;
    [SerializeField] private Sprite _inactiveImage;
    
    public void ChangeActivity(bool activity)
    {
        if(activity)
            _image.sprite = _activeImage;
        else
            _image.sprite = _inactiveImage;
    }
}
