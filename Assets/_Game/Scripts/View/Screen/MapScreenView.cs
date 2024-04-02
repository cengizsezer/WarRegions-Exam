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

public class MapScreenView : BaseScreen
{
    public Canvas ParentCanvas;
    public AttackButtonView _attackButtonview;
    [SerializeField] private ManaCurrencyView manaCurrencyView;
    [SerializeField] private CoinCurrencyView coinCurrencyView;
    [SerializeField] private MoveCountView _moveCountView;
    [SerializeField] public SafeAreaSetter safeAreaSetter;
    [SerializeField] private Canvas currencies;
    [SerializeField] private GameObject _shopButton;
    
    private GraphicRaycaster[] _graphicRaycasters;
  
    #region Injection

   
    private SignalBus _signalBus;
    private PopupService _popupService;
    private FlagService _flagService;
    private ScreenController _screenController;
    private BoardGamePlayController _boardGamePlayController;
    private EnemyMobController _enemyMobController;
    private EnemySpawnController _enemySpawnController;
    private CurrencyController _currencyController;

    [Inject]
    private void Construct
    (
       
         SignalBus signalBus
        , PopupService popupService,
        ScreenController screenController,
        FlagService flagService
        ,BoardGamePlayController boardGamePlayController
        ,EnemyMobController enemyMobController
        , EnemySpawnController enemySpawnController
        , CurrencyController currencyController
        )
    {
        _signalBus = signalBus;
        _flagService = flagService;
        _screenController = screenController;
        _popupService = popupService;
        _boardGamePlayController = boardGamePlayController;
        _enemyMobController = enemyMobController;
        _enemySpawnController = enemySpawnController;
        _currencyController = currencyController;
    }

    #endregion

    public override void Initialize()
    {
        _signalBus.Subscribe<SettingsPopupClosedSignal>(OnSettingsPopupClosed);
        safeAreaSetter.FitScreenSizeToSafeArea();
        manaCurrencyView.Initialize();
        coinCurrencyView.Initialize();
        _currencyController.ResetCurrency(CurrencyType.Mana);
        _moveCountView.Initialize();
        _boardGamePlayController.Init();
        _enemyMobController.ManuelInit();
        _enemySpawnController.ManuelInit();
        //_screenController.ManuelInit();
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
        _popupService.AddPopup(new ShopPopupParametres
        {
            PopupPriority = PopupPriority.IMMEDIATE,
            PopupParent = _screenController.currentScreen.transform,
        });

        _signalBus.Fire<SettingsPopupOpenedSignal>();
       
    }

    private void OnSettingsPopupClosed(SettingsPopupClosedSignal signal)
    {
        _flagService.SetFlag(GlobalConsts.Flags.ShopFlag, FlagState.Available);
        currencies.sortingOrder = 195;
        _shopButton.SetActive(true);
        

    }

    public override void Dispose()
    {
        _signalBus.Unsubscribe<SettingsPopupClosedSignal>(OnSettingsPopupClosed);
    }
}
