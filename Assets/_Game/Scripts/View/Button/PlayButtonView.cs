using MyProject.Core.Enums;
using MyProject.Core.Services;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

public class PlayButtonView : BaseButtonView
{
    [SerializeField] private Transform shakeObject;

    public Transform ShakeTransform => shakeObject;

    private DG.Tweening.Tween _tween;

    private void Start()
    {
        base.Initialize(); // dummy
    }

    protected override void OnButtonClicked()
    {
        if (!_flagService.IsFlagAvailable(Flags.BoardFlag)) return;
        _flagService.SetFlag(Flags.BoardFlag, FlagState.Unavailable);
        _signalBus.Fire<PlayButtonClickSignal>();
        _flagService.SetFlag(Flags.BoardFlag, FlagState.Available);
    }

    public void TakeHit()
    {
        _tween?.Kill();

        shakeObject.localScale = Vector3.one;

        _tween = shakeObject.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).OnComplete(() => { shakeObject.DOScale(new Vector3(1f, 1f, 1f), 0.1f); });
    }

    //public async UniTaskVoid DisposeBoardView()
    //{
    //    if (_boardDataController.BoardState != BoardState.Active) return;

    //    _boardHintSystem.StopTimer();
    //    _boardCoordinateSystem.Dispose();
    //    _boardDataController.SaveBoardData();
    //    _boardRewardAreaController.DisableInventoryView();
    //    _buildingCheckController.Dispose();
    //    _orderController.Dispose();
    //    _orderTrayController.Dispose();

    //    await UniTask.Delay(10);

    //    _mainCamera.gameObject.SetActive(true);
    //    _cameraPinch.Initialize();
    //    _leanDragCameraOverride.Initialize();

    //    _boardView.Disable();
    //    UnRegisterBoardEvents();

    //    _screenController.ChangeFooterState(false);
    //    _boardDataController.BoardState = BoardState.Idle;

    //    _tutorialController.ShowView();
    //}

    //public void Disable()
    //{
    //    SwitchRaycasters(false);
    //    gameObject.SetActive(false);
    //    foreach (var gridView in _gridViews)
    //    {
    //        if (!gridView) return;
    //        gridView.DespawnItem();
    //    }
    //    _flagService.SetFlag(Flags.BoardFlag, FlagState.Available);
    //}
   

}
