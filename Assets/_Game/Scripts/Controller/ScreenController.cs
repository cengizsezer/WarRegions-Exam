using MyProject.Core.Enums;
using MyProject.Core.Settings;
using Zenject;

namespace MyProject.Core.Controllers
{
    public class ScreenController : BaseController
    {
        #region Injection

        public MainScreenView MainScreen;
        public MapScreenView MapScreen;
        public BaseScreen currentScreen;

        public ScreenController
        (     
              ApplicationPrefabSettings applicationControllerSettings
            , BaseScreen.Factory screenFactory)
        {
            _applicationControllerSettings = applicationControllerSettings;
            _screenFactory = screenFactory;
        }
        #endregion

        private ApplicationPrefabSettings _applicationControllerSettings;
        private readonly BaseScreen.Factory _screenFactory;
        private ScreenState _currentState;

        public void ChangeState(ScreenState state)
        {
            CreateState(state);
        }

        public void SetCost()
        {
            MapScreen._attackButtonview.SetButtonView();
        }

        private void CreateState(ScreenState state)
        {
            ClearScreens();
            _currentState = state;

            switch (_currentState)
            {
                case ScreenState.Loading:
                    CreateLoadingScreen();
                    break;
                case ScreenState.MainMenu:
                    CreateMainScreen();
                    break;
                case ScreenState.Map:
                    CreateMapScreen();
                    break;
            }

            _signalBus.Fire(new ScreenStateChangedSignal
            {
                CurrentScreenState = _currentState
            });
        }

        private void CreateLoadingScreen()
        {

        }

        private void CreateMainScreen()
        {
            MainScreen = (MainScreenView)_screenFactory.Create(_applicationControllerSettings.MainScreenPrefab);
            currentScreen = MainScreen;
            MainScreen.Initialize();
        }

        private void CreateMapScreen()
        {
            MapScreen = (MapScreenView)_screenFactory.Create(_applicationControllerSettings.MapScreenPrefab);
            currentScreen = MapScreen;
            MapScreen.Initialize();
        }

        public void ClearScreens()
        {
            if (!currentScreen) return;
            currentScreen.DestroyView();
            currentScreen = null;
        }

        public void ChangeCurrentScreenView(bool state)
        {
            if (currentScreen != null) currentScreen.gameObject.SetActive(state);
        }

        public void CloseMapScreen()
        {
            MapScreen.gameObject.SetActive(false);
        }

        public void ManuelInit()
        {
            
        }
        protected override void OnInitialize()
        {
            _signalBus.Subscribe<LevelFailSignal>(CloseMapScreen);
            _signalBus.Subscribe<LevelSuccessSignal>(CloseMapScreen);
        }

        protected override void OnApplicationReadyToStart()
        {

        }
        protected override void OnDispose()
        {
            _signalBus.TryUnsubscribe<LevelFailSignal>(CloseMapScreen);
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(CloseMapScreen);
        }
    }
}



