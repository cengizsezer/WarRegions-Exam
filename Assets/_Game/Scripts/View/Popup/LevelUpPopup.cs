using MyProject.Core.Controllers;
using MyProject.Core.Enums;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

public class LevelUpPopup : BasePopup<LevelUpPopupParameters>
{
    #region Injection
    
    private ScreenController _screenController;
    private FlagService _flagService;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(
         ScreenController screenController
        ,FlagService flagService
        , SignalBus signalBus
      )
    {
        _screenController = screenController;
        _flagService = flagService;
        _signalBus = signalBus;
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
