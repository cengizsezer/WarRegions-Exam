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
    public List<GridView> lsAllGridView = new();

    #region Injection

    private Camera _uiCamera;
    private FlagService _flagService;
    private BoardDataController _boardDataController;

    [Inject]
    private void Construct([Inject(Id = "uiCamera")] Camera uiCamera
        , FlagService flagService
        , BoardDataController boardDataController)
    {
        _uiCamera = uiCamera;
        _flagService = flagService;
        _boardDataController = boardDataController;
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
       
    }

    public void Disable()
    {
        BoardRegion.Clear();
    }

    public override void Dispose()
    {
        _signalBus.TryUnsubscribe<LevelFailSignal>(Disable);
        _signalBus.TryUnsubscribe<LevelSuccessSignal>(Disable);
    }
}
