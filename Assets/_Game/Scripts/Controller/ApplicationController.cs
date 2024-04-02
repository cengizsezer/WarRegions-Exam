using MyProject.Core.Enums;
using MyProject.Core.Settings;
using UnityEngine;
using Zenject;

namespace MyProject.Core.Controllers
{
    public class ApplicationController : IInitializable
    {
        private readonly ApplicationPrefabSettings _prefabSettings;
        private readonly IGameLogger _logger;
        private readonly ScreenController _screenController;
        private readonly CurrencyController _currencyController;
        private readonly SignalBus _signalBus;
        private readonly LoadingProgressBarView _loadingProgressBarView;
        private readonly UserController _playerController;


        public ApplicationController(
            ApplicationPrefabSettings settings
            , IGameLogger logger
            , ScreenController screenController
            , SignalBus signalBus
            , LoadingProgressBarView loadingProgressBarView
            , CurrencyController currencyController
             , UserController playerController
           
            )
        {
            _prefabSettings = settings;
            _logger = logger;
            _screenController = screenController;
            _signalBus = signalBus;
            _loadingProgressBarView = loadingProgressBarView;
            _currencyController = currencyController;
            _playerController = playerController;
        }

        public void Initialize()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 0.02f;
            Application.runInBackground = _prefabSettings.runInBackground;

            _loadingProgressBarView.Initialize();

            SetLoading((int)LoginProgressBarValue.Start);
            InitGame();
        }

        private void SetLoading(int progress)
        {
            _logger.I("Login Loading Progress : " + progress);
            _signalBus.Fire(new LoadingProgressBarSignal(progress));
        }

        private void InitGame()
        {
            _currencyController.Init();
            _playerController.Init();
            _screenController.ChangeState(ScreenState.MainMenu);
            SetLoading((int)LoginProgressBarValue.GameReady);
            _signalBus.Fire<ApplicationReadyToStartSignal>();

        }

    }
}

