using MyProject.Core.Controllers;
using MyProject.Core.Services;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MyProject.Core.Enums;
using System.Linq;
using static MyProject.Core.Const.GlobalConsts;
using MyProject.Core.Settings;
using MyProject.Core.Data;
using MyProject.GamePlay.Characters;

namespace MyProject.GamePlay.Controllers
{
    public class EnemyMobController : BaseController
    {
        public List<EnemyMobView> LsEnemyMobViews = new();
        private int _counter;

        #region Injection
        private CoroutineService _coroutineService;
        private CoroutineHandle _coroutineHandle;
        private EnemyMobView.Factory _enemyMobFactory;
        private BoardCoordinateSystem _boardCoordinateSystem;
        private BoardGamePlayController _boardGamePlayController;
        private EnemySpawnSettings _enemySpawnSettings;
        private CharacterItemSettings _characterSettings;
        private FlagService _flagService;

        [Inject]
        private void Construct
        (
           CoroutineService coroutineService
           , EnemyMobView.Factory enemyMobFactory
            , BoardCoordinateSystem boardCoordinateSystem
            , BoardGamePlayController boardGamePlayController
            , EnemySpawnSettings enemySpawnSettings
            , CharacterItemSettings characterSettings
            , FlagService flagService
        )
        {
            _coroutineService = coroutineService;
            _enemyMobFactory = enemyMobFactory;
            _boardCoordinateSystem = boardCoordinateSystem;
            _boardGamePlayController = boardGamePlayController;
            _enemySpawnSettings = enemySpawnSettings;
            _characterSettings = characterSettings;
            _flagService = flagService;

        }
        #endregion

        protected override void OnInitialize()
        {
            _signalBus.Subscribe<LevelFailSignal>(Disable);
            _signalBus.Subscribe<LevelSuccessSignal>(Disable);
        }

        public void ManuelInit()
        {
            _counter = 0;
            if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
        }

        public void StartSpawn()
        {
            _coroutineHandle = _coroutineService.StartCoroutine(Spawn());
            _flagService.SetFlag(Flags.BoardFlag, FlagState.Available);
        }

        public EnemyMobView GetTargetEnemy()
        {
            return LsEnemyMobViews.Where(n => n.IsAlive).FirstOrDefault();
        }

        private IEnumerator<float> Spawn()
        {
            if (_counter >= _enemySpawnSettings.MaxSpawnWaveCount)
            {
                OnStopService();
                _signalBus.Fire<WaveComplateSignal>();
                yield break;
            }

            for (int i = 0; i < _enemySpawnSettings.SingleWaveEnemyCount; i++)
            {
                BaseGridItemView enemy = GetEnemyCharacter();
                EnemyMobView View = enemy.GetComponent<EnemyMobView>();

                if(_counter== _enemySpawnSettings.MaxSpawnWaveCount - 1 && i== _enemySpawnSettings.SingleWaveEnemyCount-1)
                {
                    View.IsLastEnemy = true;
                }

                LsEnemyMobViews.Add(View);
                View.BondWithGrid(_boardCoordinateSystem.LsAllGridViews.FirstOrDefault());
                View.StartMovementRoutine(_boardCoordinateSystem.LsRoadGrids, _characterSettings.EnemyGamingSettings.MovementSpeed);
                yield return Timing.WaitForSeconds(_enemySpawnSettings.UnitSpawnDelay);
            }

            yield return Timing.WaitForSeconds(0.1f);
            _counter++;
            _signalBus.Fire<StartEnemySpawnTimerSignal>();
        }

        public void RemoveEnemyMobs(EnemyMobView enemyView)
        {
            if (!_boardGamePlayController.IsRunning) return;

            LsEnemyMobViews.Remove(enemyView);
        }

        public BaseGridItemView GetEnemyCharacter()
        {
            BaseGridItemView character = _enemyMobFactory.Create();

            character.Init(new ItemData
            {
                GridItemType = GameplayMobType.Enemy,
                ItemName = ItemName.Enemy,
                CharacterType = GetRandomCharacterType()

            });

            return character;
        }
        private CharMobType GetRandomCharacterType()
        {
            CharMobType[] allTypes = (CharMobType[])System.Enum.GetValues(typeof(CharMobType));

            int randomIndex = Random.Range(0, allTypes.Length);
            return allTypes[randomIndex];
        }
        private void Disable()
        {
            OnStopService();

            foreach (var enemy in LsEnemyMobViews)
            {
                if (!enemy) return;
                enemy.Despawn();
            }

            LsEnemyMobViews.Clear();
        }

        public void OnStopService()
        {
            if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
        }

        protected override void OnApplicationReadyToStart()
        {
            if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
        }

        protected override void OnDispose()
        {
            if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
            _signalBus.TryUnsubscribe<LevelFailSignal>(Disable);
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(Disable);
        }
    }
}

