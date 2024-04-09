using MyProject.Core.Controllers;
using MyProject.Core.Enums;
using MyProject.Core.Models;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using MyProject.GamePlay.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;
using System.Collections.Generic;
using System.Linq;
using MyProject.Core.Data;

namespace MyProject.GamePlay.Controllers
{
    public class BoardGamePlayController : BaseController
    {
        
        public bool IsRunning = false;

        private HashSet<MilitaryBaseView> _selectedView = new();
        private MilitaryBaseView _lastSelectedView;
        private Vector3 _lastClickPoint;
        private bool _isDragging;
        private bool _boardDataLoaded;
        private float _lastFingerDownTime;

        #region Injecion

        private readonly Camera _mainCamera;
        private readonly BoardView _boardView;
        private readonly InputController _inputController;
        private readonly FlagService _flagService;
        private readonly BoardCoordinateSystem _boardCoordinateSystem;
        private readonly SignalBus _signalBus;
        private readonly BoardDataController _boardDataController;
        private readonly ItemTweenSettings _itemTweenSettings;
        private readonly UserController _userController;
        private readonly CurrencyController _currencyController;


        public BoardGamePlayController
        (
            Camera mainCamera
            , BoardView boardView
            , InputController inputController
            , BoardCoordinateSystem boardCoordinateSystem
            , SignalBus signalBus
            , BoardDataController boardDataController
            , ItemTweenSettings itemTweenSettings
            , FlagService flagService
            , UserController userController
            ,CurrencyController currencyController

        )
        {
            _boardView = boardView;
            _inputController = inputController;
            _boardCoordinateSystem = boardCoordinateSystem;
            _signalBus = signalBus;
            _mainCamera = mainCamera;
            _boardDataController = boardDataController;
            _itemTweenSettings = itemTweenSettings;
            _flagService = flagService;
            _userController = userController;
            _currencyController = currencyController;
          
        }

        #endregion

        public void Init()
        {
           
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
            _flagService.SetFlag(Flags.BoardFlag, FlagState.Available);

        }

        public void DisposeBoard(ContinueButtonClickSignal signal)
        {
            ResetGame().Forget();
        }
        public async UniTaskVoid ResetGame()
        {
            if (_boardDataController.BoardState != BoardState.Active) return;
            _boardDataController.BoardState = BoardState.Idle;
            IsRunning = false;
            await UniTask.Delay(10);
            _boardCoordinateSystem.Dispose();
            _boardView.Disable();
            UnRegisterBoardEvents();
            _mainCamera.gameObject.SetActive(false);

            OnDispose();
           
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
            if (_boardDataController.BoardState == BoardState.Idle) return;
            _selectedView.Clear();
            if (Input.touchCount > 1)
            {
                return;
            }
            if (TryCatchGridView(out var view))
            {
               
                if (view.SoldierCount <= 0) return;
               

                // Eğer daha önce seçili bir asker yoksa veya son tıklamadan sonra geçen süre 0.5 saniyeden fazlaysa
                if (_selectedView.Count == 0 || Time.realtimeSinceStartup - _lastFingerDownTime >= 0.5f)
                {
                    _lastFingerDownTime = Time.realtimeSinceStartup;
                    _selectedView.Add(view);
                    view.SelectView();
                  
                }
            }
        }


        private void OnFingerUp()
        {
            
            if (_selectedView.Count == 0)
            {
                return;
            }
           
            _isDragging = false;
            _lastSelectedView = _selectedView.Last();
            TryReleaseItem();
        }


        private void TryReleaseItem()
        {
            if (_selectedView == null || _selectedView.Count == 0) return;
           
            if (TryCatchGridView(out MilitaryBaseView other))
            {
                foreach (var view in _selectedView)
                {
                    if (view == other) continue; // Aynı askere iki kez tıklama durumunu önle

                    view.SendingTroops(other);
                }
            }
           
            foreach (var view in _selectedView)
            {
                view.UnSelectView();
            }
            _selectedView.Clear();
        }
        private void TryDragSelectedItem()
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(_inputController.FingerPosition), out var hit, 100, LayerMasks.COLLIDER))
            {
                if (Vector3.Distance(_lastClickPoint, hit.point) < _itemTweenSettings.DragDeadZone && !_isDragging) return;

                if(hit.collider.TryGetComponent(out MilitaryBaseView other))
                {
                    if(other.GetUserType()==UserType.Player && other.SoldierCount>0)
                    {
                        _selectedView.Add(other);
                    }
                }
                // Seçili askerlerin seçimlerini kaldır
                foreach (var view in _selectedView)
                {
                    view.UnSelectView();
                }

                _isDragging = true;
                _lastSelectedView = null;
            }
        }
        private void OnFingerUpdate()
        {
            if (_selectedView.Count > 0)
            {
                TryDragSelectedItem();
            }
        }

        private bool TryCatchGridView(out MilitaryBaseView militaryBaseView)
        {
            militaryBaseView = null;

            if (Physics.Raycast(_mainCamera.ScreenPointToRay(_inputController.FingerPosition), out var hit, 100, LayerMasks.COLLIDER))
            {
                if (hit.collider.TryGetComponent(out MilitaryBaseView view))
                {
                    militaryBaseView = view;
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
       

        public void CheckLevelControl()
        {
            bool win = _boardCoordinateSystem.LsMilitaryBaseView.TrueForAll(n=>n.GetUserType()==UserType.Player);
            bool lose= _boardCoordinateSystem.LsMilitaryBaseView.TrueForAll(n => n.GetUserType() == UserType.Enemy);

            if(win)
            {
                CurrencyData currencyData = new CurrencyData
                {
                    CurrencyType = CurrencyType.Coin,
                    CurrencyValue = 100
                };

                _signalBus.Fire<CurrencyGainSignal>(new CurrencyGainSignal(ItemName.Coin, 100, Vector3.zero));
                
                _userController.LevelWin();
            }
            else if(lose)
            {
                _userController.LevelFail();
            }
        }
    }

}
