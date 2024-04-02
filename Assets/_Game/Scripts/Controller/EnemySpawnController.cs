using MyProject.Core.Services;
using MyProject.Core.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.GamePlay.Controllers
{
    public class EnemySpawnController : IInitializable, IDisposable, ITimerOwner
    {
        public bool IsWaveComplate = false;

        private int _counter;
        private bool _timerActive;

        #region Injection

        private readonly TimerService _timerService;
        private readonly SignalBus _signalBus;
        private readonly CoroutineService _coroutineService;
        private readonly EnemyMobController _enemyMobController;
        private readonly EnemySpawnSettings _enemySpawnSettings;

        [Inject]
        public EnemySpawnController
        (
              TimerService timerService
            , SignalBus signalBus
            , BoardCoordinateSystem boardCoordinateSystem
            , CoroutineService coroutineService
            , EnemyMobController enemyMobController
            , EnemySpawnSettings enemySpawnSettings
        )
        {
            _timerService = timerService;
            _signalBus = signalBus;
            _coroutineService = coroutineService;
            _enemyMobController = enemyMobController;
            _enemySpawnSettings = enemySpawnSettings;
        }

        #endregion

        public void ManuelInit()
        {
            _counter = 0;
        }
       
        public void Initialize()
        {
            _signalBus.Subscribe<StartEnemySpawnTimerSignal>(StartTimer);
            _signalBus.Subscribe<StopEnemySpawnTimerSignal>(StopTimer);
            _signalBus.Subscribe<LevelSuccessSignal>(StopTimer);
            _signalBus.Subscribe<LevelFailSignal>(StopTimer);
            _signalBus.Subscribe<WaveComplateSignal>(StopTimer);
        }

        public void StartTimer()
        {
            if (_timerActive) return;
            _counter = (int)_enemySpawnSettings.WaveDelay;
            _timerService.Subscribe(this);
            _timerActive = true;
            IsWaveComplate = true;
        }

        public void StopTimer()
        {
            _counter = (int)_enemySpawnSettings.WaveDelay;
            _timerService.UnSubscribe(this);
            _timerActive = false;
        }

        public void OnStopMission()
        {
            StopTimer();
        }

        public void UpdateTime(float intervalTime)
        {
            switch (_counter)
            {
                case > 0:
                    _counter--;
                    break;
                case 0:
                    _counter = (int)_enemySpawnSettings.WaveDelay;
                    TrySpawn();
                    StopTimer();
                    break;
            }
        }

        private void TrySpawn()
        {
            _enemyMobController.StartSpawn();
        }

        public void ResumeTime(float passedRealtime)
        {

        }

        public void Dispose()
        {
            StopTimer();
            _signalBus.Unsubscribe<StartEnemySpawnTimerSignal>(StartTimer);
            _signalBus.Unsubscribe<StopEnemySpawnTimerSignal>(StopTimer);
            _signalBus.Unsubscribe<LevelSuccessSignal>(StopTimer);
            _signalBus.Unsubscribe<LevelFailSignal>(StopTimer);
            _signalBus.Unsubscribe<WaveComplateSignal>(StopTimer);
        }
    }
}


