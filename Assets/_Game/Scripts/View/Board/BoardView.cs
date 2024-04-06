using MyProject.Core.Enums;
using MyProject.Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

public class BoardView : BaseView
{
    public List<ColorRegion> BoardRegion = new();

    public Transform _dragParent;

    #region Injection

    private Camera _uiCamera;
    private FlagService _flagService;

    [Inject]
    private void Construct([Inject(Id = "uiCamera")] Camera uiCamera
        , FlagService flagService)
    {
        _uiCamera = uiCamera;
        _flagService = flagService;
    }

    #endregion
    public override void Initialize()
    {
        _signalBus.Subscribe<LevelFailSignal>(Disable);
        _signalBus.Subscribe<LevelSuccessSignal>(Disable);
    }
   
    public void Init()
    {
        gameObject.SetActive(true);
        _flagService.SetFlag(Flags.BoardFlag, FlagState.Available);
    }

    public void Disable()
    {
       
        //foreach (var gridView in _gridViews)
        //{
        //    if (!gridView) return;
        //    gridView.DespawnItem();
        //}

        gameObject.SetActive(false);
       
    }

    public override void Dispose()
    {
        _signalBus.TryUnsubscribe<LevelFailSignal>(Disable);
        _signalBus.TryUnsubscribe<LevelSuccessSignal>(Disable);
    }
}
