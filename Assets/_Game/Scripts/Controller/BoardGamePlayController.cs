using MyProject.Core.Controllers;
using MyProject.Core.Enums;
using MyProject.Core.Models;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using MyProject.GamePlay.Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.GamePlay.Controllers
{
    public class BoardGamePlayController : BaseController
    {
        public bool IsRunning = false;

        private MillitaryBaseView _selectedView;
        private MillitaryBaseView _lastSelectedView;
        private Vector3 _lastClickPoint;
        private bool _isDragging;
        private bool _boardDataLoaded;
        private float _lastFingerDownTime;

        #region Injecion

        private readonly Camera _uiCamera;
        private readonly Camera _mainCamera;
        private readonly BoardView _boardView;
        private readonly InputController _inputController;
        private readonly CurrencyModel _currencyModel;
        private readonly TaskService _taskService;
        private readonly FlagService _flagService;
        private readonly BoardCoordinateSystem _boardCoordinateSystem;
        private readonly ScreenController _screenController;
        private readonly SignalBus _signalBus;
        private readonly UserModel _userModel;
        private readonly CurrencyController _currencyController;
        private readonly BoardDataController _boardDataController;
        private readonly ItemTweenSettings _itemTweenSettings;
        private readonly PlayerMobController _playerMobController;
        private readonly UserController _userController;
        
        public BoardGamePlayController
        (
            [Inject(Id = "uiCamera")] Camera uiCamera
            , Camera mainCamera
            , BoardView boardView
            , InputController inputController
            , CurrencyModel currencyModel
            , TaskService taskService
            , BoardCoordinateSystem boardCoordinateSystem
            , ScreenController screenController
            , UserModel userModel
            , CurrencyController currencyController
            , SignalBus signalBus
            , BoardDataController boardDataController
            , ItemTweenSettings itemTweenSettings
            , PlayerMobController playerMobController
            , FlagService flagService
            , UserController userController
           

        )
        {
            _uiCamera = uiCamera;
            _boardView = boardView;
            _inputController = inputController;
            _currencyModel = currencyModel;
            _taskService = taskService;
            _boardCoordinateSystem = boardCoordinateSystem;
            _screenController = screenController;
            _userModel = userModel;
            _currencyController = currencyController;
            _signalBus = signalBus;
            _mainCamera = mainCamera;
            _boardDataController = boardDataController;
            _itemTweenSettings = itemTweenSettings;
            _playerMobController = playerMobController;
            _flagService = flagService;
            _userController = userController;
          
        }

        #endregion

        public void Init()
        {
            Debug.Log("Init");
            InitBoardView().Forget();
            _signalBus.Subscribe<ContinueButtonClickSignal>(DisposeBoard);
            _signalBus.Fire<StartEnemySpawnTimerSignal>();
        }

        public async UniTaskVoid InitBoardView()
        {
            await UniTask.Delay(10);
            IsRunning = true;

            _boardCoordinateSystem.Init();
            _boardView.Init();
            RegisterBoardEvents();
            _mainCamera.gameObject.SetActive(true);
            _boardDataController.BoardState = BoardState.Active;
           
        }

        public void DisposeBoard(ContinueButtonClickSignal signal)
        {
            ResetGame().Forget();
        }

        public async UniTaskVoid DisposeBoardView()
        {
            if (_boardDataController.BoardState != BoardState.Active) return;

            _boardCoordinateSystem.Dispose();
            await UniTask.Delay(10);
            _mainCamera.gameObject.SetActive(false);
            _boardView.Disable();
            UnRegisterBoardEvents();
            _boardDataController.BoardState = BoardState.Idle;
        }

        public async UniTaskVoid ResetBoard()
        {
            if (_boardDataController.BoardState != BoardState.Active) return;

            _boardCoordinateSystem.Dispose();
            _boardView.Disable();
            UnRegisterBoardEvents();

            _boardDataController.BoardState = BoardState.Idle;

            await UniTask.Delay(10);
        }

        private void RegisterBoardEvents()
        {
            _signalBus.Subscribe<FingerDownSignal>(OnFingerDown);
            _signalBus.Subscribe<FingerUpdateSignal>(OnFingerUpdate);
            _signalBus.Subscribe<FingerUpSignal>(OnFingerUp);
        }

        private void UnRegisterBoardEvents()
        {
            _signalBus.TryUnsubscribe<FingerDownSignal>(OnFingerDown);
            _signalBus.TryUnsubscribe<FingerUpdateSignal>(OnFingerUpdate);
            _signalBus.TryUnsubscribe<FingerUpSignal>(OnFingerUp);
        }

        private void OnFingerDown()
        {
          
            if (!_flagService.IsFlagAvailable(Flags.BoardFlag)) return;

            if (TryCatchGridView(out var view))
            {

                switch (view.MilitaryBaseType)
                {
                    case MilitaryBaseType.Land:

                        if (_lastSelectedView)
                        {
                            if (Time.realtimeSinceStartup - _lastFingerDownTime < 0.5f)
                            {
                                return;
                            }

                            
                        }
                      
                        _lastFingerDownTime = Time.realtimeSinceStartup;
                        _selectedView = view;
                        _selectedView.SelectView();
                        break;

                    case MilitaryBaseType.Sea:

                        if (_lastSelectedView)
                        {
                            if (Time.realtimeSinceStartup - _lastFingerDownTime < 0.5f)
                            {
                                return;
                            }


                        }
                       
                        _lastFingerDownTime = Time.realtimeSinceStartup;
                        _selectedView = view;
                        _selectedView.SelectView();
                        break;
                }
            }

        }
       
        private void OnFingerUp()
        {
            if (!_selectedView)
            {
                return;
            }

            _isDragging = false;
            _lastSelectedView = _selectedView;
            TryReleaseItem();
        }
       
        private void TryReleaseItem()
        {
            if (_selectedView == null)
            {
                return;
            }

            if (TryCatchGridView(out MillitaryBaseView other))
            {

                if (other == _selectedView)
                {
                    return;
                }

                switch (other.UserType)
                {
                    case UserType.Player:
                        Debug.Log("Player finger down");
                        break;
                    case UserType.Enemy:

                        if (_selectedView.UserType != UserType.Player)
                            return;

                        _selectedView.SendingTroops(other);
                        _selectedView = null;
                        Debug.Log("Enemy finger down");
                        break;
                    default:
                        break;
                }
            }

        }
        private void TryDragSelectedItem()
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(_inputController.FingerPosition), out var hit, 100,
                    LayerMasks.COLLIDER))
            {
                if (Vector3.Distance(_lastClickPoint, hit.point) < _itemTweenSettings.DragDeadZone &&
                    !_isDragging) return;
             

                _selectedView.UnSelectView();
                _isDragging = true;
                _lastSelectedView = null;
            }

        }
        private void OnFingerUpdate()
        {
            if (_selectedView)
            {
                TryDragSelectedItem();
            }
        }
       
        private bool TryCatchGridView(out MillitaryBaseView millitaryBaseView)
        {
            millitaryBaseView = null;

            if (Physics.Raycast(_mainCamera.ScreenPointToRay(_inputController.FingerPosition), out var hit, 100,
                    LayerMasks.COLLIDER))
            {
                if (hit.collider.TryGetComponent(out MillitaryBaseView view))
                {

                    millitaryBaseView = view;
                    _lastClickPoint = hit.point;
                    return true;
                }
            }

            return false;
        }
       

        protected override void OnInitialize()
        {

        }

        protected override void OnApplicationReadyToStart()
        {
        }

        protected override void OnDispose()
        {
            _signalBus.TryUnsubscribe<ContinueButtonClickSignal>(DisposeBoard);
            UnRegisterBoardEvents();
        }
        public async UniTaskVoid ResetGame()
        {
            if (_boardDataController.BoardState != BoardState.Active) return;
            _boardDataController.BoardState = BoardState.Idle;
            IsRunning = false;
            _boardCoordinateSystem.Dispose();
            _boardView.Disable();
            UnRegisterBoardEvents();
            _mainCamera.gameObject.SetActive(false);

            OnDispose();
            await UniTask.Delay(10);
        }

    }

}
