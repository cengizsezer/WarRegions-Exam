using MyProject.Core.Controllers;
using MyProject.Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BaseButtonView : BaseView
{
    [SerializeField] protected Button _button;
    [SerializeField] CanvasGroup _canvasGroup;
    //[SerializeField] UIPosYSetter _uiPosYSetter;

    [HideInInspector] public bool IsButtonVisible;
    [HideInInspector] public bool IsButtonAnimationActive;

    #region Injection

    protected PopupService _popupService;
    protected SignalBus _signalBus;
    //protected AudioService _audioService;
    protected FlagService _flagService;


    [Inject]
    private void Construct(PopupService popupService
        //, AudioService audioService
        , SignalBus signalBus
        , ScreenController screenController
        , FlagService flagService)
    {
        _popupService = popupService;
        _signalBus = signalBus;
        //_audioService = audioService;
        _flagService = flagService;
    }

    #endregion

    public override void Initialize()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void SetButtonVisible(bool visible)
    {
        _canvasGroup.alpha = visible ? 1 : 0;
        _canvasGroup.interactable = visible;
        _canvasGroup.blocksRaycasts = visible;
        IsButtonVisible = visible;
    }

    protected void SetButtonAnim(bool active)
    {
        //_button.transition = active ? Transition.SpriteSwap : Transition.None;
        //_uiPosYSetter.Override = !active;
        IsButtonAnimationActive = active;
    }

    protected virtual void OnButtonClicked()
    {

    }

    public override void Dispose()
    {
        _button.onClick.RemoveListener(OnButtonClicked);

        base.Dispose();
    }
}
