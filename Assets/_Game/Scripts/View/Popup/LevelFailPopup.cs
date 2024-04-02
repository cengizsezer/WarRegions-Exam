using MyProject.Core.Const;
using MyProject.Core.Controllers;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

public class LevelFailPopup : BasePopup<LevelFailPopupParameters>
{
    #region Injection
   
    private ScreenController _screenController;
    private SignalBus _signalBus;
    private FlagService _flagService;

    [Inject]
    private void Construct(
         ScreenController screenController
        ,SignalBus signalBus
        ,FlagService flagService
        )
    {
        _screenController = screenController;
        _signalBus = signalBus;
        _flagService = flagService;


    }

    #endregion

    public override void Show()
    {
        base.Show();
    }

    public override void ClosePopup()
    {
        if (!_flagService.IsFlagAvailable(Flags.BoardFlag)) return;
        _flagService.SetFlag(Flags.BoardFlag, FlagState.Unavailable);

        _screenController.ChangeState(ScreenState.MainMenu);
        _signalBus.Fire<ContinueButtonClickSignal>();
        base.ClosePopup();
    }
}
public class LevelFailPopupParameters : BasePopupParameters
{
    public UserLevelData PlayerLevelData;
    public int Level;

    public override bool IsBundleRequired()
    {
        return false;
    }

    public override string PopupName()
    {
        return GlobalConsts.PopupName.LevelFailPopup;
    }

    public override float CloseDuration()
    {
        return 0f;
    }
}
