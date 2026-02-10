using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePopup : MonoBehaviour
{
    [SerializeField] protected Button _exitButton;

    public event Action CloseClick;

    private UnityEngine.Events.UnityAction _onExit;

    protected virtual void Awake()
    {
        _onExit = () => CloseClick?.Invoke();
        _exitButton.onClick.AddListener(_onExit);


    }



    protected virtual void OnDestroy()
    {
        if (_onExit != null)
            _exitButton.onClick.RemoveListener(_onExit);
    }
}