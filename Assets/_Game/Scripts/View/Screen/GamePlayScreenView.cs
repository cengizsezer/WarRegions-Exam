using MyProject.Core.Const;
using MyProject.Core.Controllers;
using MyProject.Core.Enums;
using MyProject.Core.Services;
using MyProject.GamePlay.Controllers;
using Cysharp.Threading.Tasks;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

public class GamePlayScreenView : BaseScreen
{
    public Canvas ParentCanvas;
   
    [SerializeField] private CoinCurrencyView coinCurrencyView;
    [SerializeField] public SafeAreaSetter safeAreaSetter;
    [SerializeField] private Canvas currencies;
    
    private GraphicRaycaster[] _graphicRaycasters;
  
    #region Injection

   
    private SignalBus _signalBus;
    private PopupService _popupService;
    private FlagService _flagService;
    private ScreenController _screenController;
    private BoardGamePlayController _boardGamePlayController;
    private CurrencyController _currencyController;

    [Inject]
    private void Construct
    (
       
         SignalBus signalBus
        , PopupService popupService,
        ScreenController screenController,
        FlagService flagService
        ,BoardGamePlayController boardGamePlayController
        , CurrencyController currencyController
        )
    {
        _signalBus = signalBus;
        _flagService = flagService;
        _screenController = screenController;
        _popupService = popupService;
        _boardGamePlayController = boardGamePlayController;
        _currencyController = currencyController;
    }

    #endregion

    public override void Initialize()
    {
        _signalBus.Subscribe<SettingsPopupClosedSignal>(OnSettingsPopupClosed);
        safeAreaSetter.FitScreenSizeToSafeArea();
        coinCurrencyView.Initialize();
        _currencyController.ResetCurrency(CurrencyType.Mana);
        _boardGamePlayController.Init();
        currencies.sortingOrder = 195;
    }

    public void SwitchRaycasters(bool state)
    {
        if (_graphicRaycasters == null || _graphicRaycasters.Length == 0)
        {
            _graphicRaycasters = GetComponentsInChildren<GraphicRaycaster>();
        }

        foreach (var raycaster in _graphicRaycasters)
        {
            raycaster.enabled = state;
        }
    }

    public void ShowSettingsScreen()
    {
        if (!_flagService.IsFlagAvailable(GlobalConsts.Flags.ShopFlag))
        {
            return;
        }
        currencies.sortingOrder = 90;

        _signalBus.Fire<SettingsPopupOpenedSignal>();
       
    }

    private void OnSettingsPopupClosed(SettingsPopupClosedSignal signal)
    {
        _flagService.SetFlag(GlobalConsts.Flags.ShopFlag, FlagState.Available);
        currencies.sortingOrder = 195;
        

    }

    public override void Dispose()
    {
        _signalBus.Unsubscribe<SettingsPopupClosedSignal>(OnSettingsPopupClosed);
    }
}
