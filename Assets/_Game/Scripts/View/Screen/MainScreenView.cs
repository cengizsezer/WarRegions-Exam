using MyProject.Core.Const;
using MyProject.Core.Controllers;
using MyProject.Core.Enums;
using MyProject.Core.Services;
using Cysharp.Threading.Tasks;
using Lean.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;


public class MainScreenView : BaseScreen
{
    
    [SerializeField] public SafeAreaSetter safeAreaSetter;
    [SerializeField] private Canvas _currencies;
    [SerializeField] private GameObject _settingsButton;
   
   
    #region Injection

   
    private  Camera _mainCamera;
    private SignalBus _signalBus;
    private PopupService _popupService;
    private FlagService _flagService;
    private ScreenController _screenController;

    [Inject]
    private void Construct
    (
       
        Camera mainCamera
        ,SignalBus signalBus
        ,PopupService popupService,
        ScreenController screenController,
        FlagService flagService)
    {
        _signalBus = signalBus;
        _mainCamera = mainCamera;
        _flagService = flagService;
        _screenController = screenController;
        _popupService = popupService;


    }

    #endregion

    public override void Initialize()
    {
        _signalBus.Subscribe<SettingsPopupClosedSignal>(OnSettingsPopupClosed);
        _signalBus.Subscribe<PlayButtonClickSignal>(OnPlayButtonClicked);
        safeAreaSetter.FitScreenSizeToSafeArea();
        _currencies.sortingOrder = 195;
    }
    
    public void ShowSettingsScreen()
    {

        if (!_flagService.IsFlagAvailable(Flags.ShopFlag))
        {
            return;
        }
        _currencies.sortingOrder = 90;
        _popupService.AddPopup(new ShopPopupParametres
        {
            PopupPriority = PopupPriority.IMMEDIATE,
            PopupParent = _screenController.MainScreen.transform,
        });

        _signalBus.Fire<SettingsPopupOpenedSignal>();
       
    }
    private void OnSettingsPopupClosed(SettingsPopupClosedSignal signal)
    {
        _flagService.SetFlag(Flags.ShopFlag, FlagState.Available);
        _currencies.sortingOrder = 195;
        _settingsButton.SetActive(true);

    }
    private async UniTaskVoid DisposeMainMenu()
    {
        await UniTask.Delay(10);
        Disable();
        _mainCamera.gameObject.SetActive(true);
       
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        _screenController.ChangeState(ScreenState.Map);
        _flagService.SetFlag(Flags.BoardFlag, FlagState.Available);
    }
    private void OnPlayButtonClicked()
    {
        DisposeMainMenu().Forget();
    }

    public override void Dispose()
    {
        _signalBus.Unsubscribe<PlayButtonClickSignal>(OnPlayButtonClicked);
        _signalBus.Unsubscribe<SettingsPopupClosedSignal>(OnSettingsPopupClosed);
    }
}

