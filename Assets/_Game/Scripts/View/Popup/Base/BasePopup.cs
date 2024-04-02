using MyProject.Core.Controllers;
using MyProject.Core.Enums;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

public class BasePopup<T> : MonoBehaviour, IBasePopup where T : BasePopupParameters
{
    [SerializeField] protected Animator _animator;

    protected T _parameters;
    private CoroutineHandle _popupCloseAnimCoroutineHandle;

    #region Injection

    protected PopupService _popupService;
    private PopupSettings _settings;
    private CoroutineService _coroutineService;
    // private AudioService _audioService;
    private ScreenController _screenController;

    [Inject]
    private void Construct(PopupService popupService
        , PopupSettings settings
        , CoroutineService coroutineService
        //, AudioService audioService
        , ScreenController screenController)
    {
        _popupService = popupService;
        _settings = settings;
        _coroutineService = coroutineService;
        //_audioService = audioService;
        _screenController = screenController;
    }

    #endregion

    /// <summary>
    /// Every popup is initialized with it's own parameters
    /// </summary>
    /// <param name="parameters">Parameters specific to popup that enables to transfer custom requests when opening popup</param>
    public void Init(BasePopupParameters parameters)
    {
        _parameters = (T)parameters;

        if (_parameters.ShouldDisableUI) _screenController.ChangeCurrentScreenView(false);

        var canvas = gameObject.GetComponentInChildren<Canvas>();
        if (canvas == null) return;

        canvas.planeDistance = _parameters.OverridePlaneDistance ? _parameters.PlaneDistance : canvas.planeDistance;
    }

    /// <summary>
    /// Shows the popup with it's initialized parameters
    /// </summary>
    public virtual void Show()
    {

    }

    /// <summary>
    /// Starts the regular closing schedule of the popup.
    /// </summary>
    public virtual void ClosePopup()
    {
        if (_parameters.PopupPriority == PopupPriority.NORMAL)
        {
            // _audioService.PlaySfx(SfxName.Buttons);
            _popupCloseAnimCoroutineHandle = _coroutineService.StartCoroutine(SetAnimationBeforeClose());
        }

        if (_parameters.PopupPriority == PopupPriority.IMMEDIATE)
        {
            Remove();
        }
    }

    /// <summary>
    /// Starts the regular closing schedule of the popup without animation.
    /// </summary>
    public virtual void ClosePopupWithoutAnimation()
    {
        Remove();
    }

    private IEnumerator<float> SetAnimationBeforeClose()
    {
        if (_animator != null)
        {
            _animator.SetTrigger(Animations.Disappear);

        }
        yield return Timing.WaitForSeconds(_parameters.CloseDuration());
        Remove();
    }

    /// <summary>
    /// Removes the popup from PopupService and closes & destroys the popup
    /// </summary>
    protected void Remove()
    {
        if (_parameters.PopupPriority == PopupPriority.IMMEDIATE)
        {
            _popupService.RemoveImmediatePopup();
        }
        else
        {
            _popupService.RemoveNormalPopup(_parameters);
        }
    }

    /// <summary>
    /// This method is called right before the popup is destroyed
    /// </summary>
    protected virtual void OnPopupDestroyed()
    {

    }

    public void DestroyPopup(Action destroyCallback)
    {
        if (_parameters == null) return;
        if (_popupCloseAnimCoroutineHandle.IsRunning) _coroutineService.StopCoroutine(_popupCloseAnimCoroutineHandle);

        if (_parameters.Task != null)
        {
            _parameters.Task.CompleteTask();
        }

        destroyCallback?.Invoke();
        OnPopupDestroyed();
        Destroy(gameObject);

        if (_parameters.ShouldDisableUI) _screenController.ChangeCurrentScreenView(true);

        _parameters = null;
    }
}
