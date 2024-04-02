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

        private GridView _selectedGrid;
        private GridView _lastSelectedGrid;
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
            ,UserController userController

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
            InitBoardView().Forget();
            _signalBus.Subscribe<AttackButtonClickSignal>(GetRandomCharacter);
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
            _flagService.SetFlag(Flags.BoardFlag, FlagState.Unavailable);
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

            if (TryCatchGridView(out var gridView))
            {

                switch (gridView.GridState)
                {
                    case GridState.Filled:
                        if (_lastSelectedGrid)
                        {
                            if (Time.realtimeSinceStartup - _lastFingerDownTime < 0.5f
                                && _lastSelectedGrid == gridView
                                && _lastSelectedGrid.PlayerCharacter != null
                                )
                            {
                                return;
                            }

                            _lastSelectedGrid.UnSelectGrid();
                        }

                        _lastFingerDownTime = Time.realtimeSinceStartup;
                        _selectedGrid = gridView;
                        _selectedGrid.GridState = GridState.Free;

                        if (_selectedGrid.PlayerCharacter)
                        {
                            _selectedGrid.PlayerCharacter.OnSelected(_selectedGrid);
                            _selectedGrid.PlayerCharacter.transform.SetParent(_boardView._dragParent);
                        }

                        _selectedGrid.SelectGrid(false);
                        break;

                    case GridState.Free:
                        break;
                }
            }

        }
        public void GetRandomCharacter(AttackButtonClickSignal signal)
        {
            PurchaseForNewCharacter();
        }
        private bool PurchaseForNewCharacter()
        {
            if (!IsRunning) return false;

            if (!_flagService.IsFlagAvailable(Flags.PurchaseFlag)) return false;

            _flagService.SetFlag(Flags.PurchaseFlag, FlagState.Unavailable);

            _taskService.AddTask(new LockUIGameTask());


            var currencyData = _currencyController.GetCurrencyData(ItemName.Mana, (int)_currencyController.CurrentCostManaValue);

            if (!_currencyController.TryConsume(currencyData))
            {
                Debug.Log("Para birimi tüketilemedi. İşlem durduruldu.");
                return false; 
            }
         
            _currencyController.IncreaseCurrentManaCost();
            GridView gridView = _boardCoordinateSystem.GetRandomGridWithNullConnected();
            gridView.PlayerCharacter = _playerMobController.GetRandomCharacter();
            PlayerMobView playerMobView = gridView.PlayerCharacter.GetComponent<PlayerMobView>();
            _playerMobController.LsPlayerMobViews.Add(playerMobView);
            gridView.PlayerCharacter.BondWithGrid(gridView);

            var manaPurchaseTask = new UpdateCurrencyGameTask();
            manaPurchaseTask.Initialize(currencyData, CurrencyUpdateType.Consume);
            _taskService.AddTask(manaPurchaseTask);


            _taskService.AddTask(new UnlockUIGameTask());
            _flagService.SetFlag(Flags.PurchaseFlag, FlagState.Available);


            return true;
        }
        private void OnFingerUp()
        {
            if (!_selectedGrid || !_selectedGrid.PlayerCharacter)
            {
                return;
            }

            _isDragging = false;
            TryInteractItem();
            _lastSelectedGrid = _selectedGrid;
            TryReleaseItem();
        }
        private void TryInteractItem()
        {
            var grid = _selectedGrid;
            var gridItem = grid.PlayerCharacter;
        }
        private void TryReleaseItem()
        {
            if (_selectedGrid == null)
            {
                return;
            }

            if (TryCatchGridView(out GridView otherGrid))
            {

                if (otherGrid == _selectedGrid)
                {
                    ResetSelectedItemPosition();

                    return;
                }

                switch (otherGrid.GridState)
                {
                    case GridState.Free:
                        //LandSelectedItemToFreeGrid(otherGrid);
                        ResetSelectedItemPosition();
                        otherGrid.SelectGrid(false);
                        break;
                    case GridState.Filled when IsGridMergeable(otherGrid):
                        MergeSelectedItemWith(otherGrid);
                        otherGrid.SelectGrid(true);
                        break;
                    case GridState.Filled when !IsGridMergeable(otherGrid):
                        ResetSelectedItemPosition();
                        otherGrid.SelectGrid(true);
                        break;
                    default:
                        ResetSelectedItemPosition();
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
                if (!_selectedGrid.PlayerCharacter) return;

                _selectedGrid.UnSelectGrid();
                _selectedGrid.PlayerCharacter.OnDragged(hit.point);
                _isDragging = true;
                _lastSelectedGrid = null;
            }

        }

        private void OnFingerUpdate()
        {
            if (_selectedGrid)
            {
                TryDragSelectedItem();
            }
        }

        private void MergeSelectedItemWith(GridView otherGrid)
        {
            otherGrid.GridState = GridState.Filled;
            var selectedItem = _selectedGrid.PlayerCharacter;


            otherGrid.PlayerCharacter.Despawn();
            otherGrid.PlayerCharacter = selectedItem;
            selectedItem.OnMerged(otherGrid);

            _lastSelectedGrid = otherGrid;
            _selectedGrid.PlayerCharacter = null;
            _selectedGrid = null;


        }

        private void ResetSelectedItemPosition()
        {
            _selectedGrid.PlayerCharacter.MoveToGrid(_selectedGrid, _itemTweenSettings.RushDuration);
            _selectedGrid.GridState = GridState.Filled;
            _selectedGrid.SelectGrid(true);
            _lastSelectedGrid = _selectedGrid;
            _selectedGrid = null;
        }

        private bool TryCatchGridView(out GridView grid)
        {
            grid = null;

            if (Physics.Raycast(_mainCamera.ScreenPointToRay(_inputController.FingerPosition), out var hit, 100,
                    LayerMasks.COLLIDER))
            {
                if (hit.collider.TryGetComponent(out GridView gridView))
                {

                    grid = gridView;
                    _lastClickPoint = hit.point;
                    return true;
                }
            }

            return false;
        }

        private bool IsGridMergeable(GridView otherGrid)
        {
            return IsItemSame(otherGrid) && IsGridLevelSame(otherGrid) && IsCharacterTypeSame(otherGrid) && !AreGridsMaxLevel(otherGrid);
        }

        private bool IsItemSame(GridView otherGrid)
        {
            if (!_selectedGrid.PlayerCharacter || !otherGrid.PlayerCharacter) return false;
            return _selectedGrid.PlayerCharacter.ItemName == otherGrid.PlayerCharacter.ItemName;
        }

        private bool IsCharacterTypeSame(GridView otherGrid)
        {
            if (!_selectedGrid.PlayerCharacter || !otherGrid.PlayerCharacter) return false;
            return _selectedGrid.PlayerCharacter.CharacterType == otherGrid.PlayerCharacter.CharacterType;
        }

        private bool IsGridLevelSame(GridView otherGrid)
        {
            if (!_selectedGrid.PlayerCharacter || !otherGrid.PlayerCharacter) return false;
            return _selectedGrid.PlayerCharacter.ItemLevel == otherGrid.PlayerCharacter.ItemLevel;
        }

        private bool AreGridsMaxLevel(GridView otherGrid)
        {
            if (!_selectedGrid.PlayerCharacter || !otherGrid.PlayerCharacter) return false;
            return _selectedGrid.PlayerCharacter.IsMaxLevel() || otherGrid.PlayerCharacter.IsMaxLevel();
        }

        protected override void OnInitialize()
        {

        }

        protected override void OnApplicationReadyToStart()
        {
        }

        protected override void OnDispose()
        {
            _signalBus.TryUnsubscribe<AttackButtonClickSignal>(GetRandomCharacter);
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
