using MEC;
using MyProject.Core.Controllers;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using MyProject.GamePlay.Characters;
using MyProject.GamePlay.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyMobController : BaseController
{

    #region Injection
    private CoroutineService _coroutineService;
    private CoroutineHandle _coroutineHandle;
    private MobView.Factory _mobFactory;
    private EnemySpawnSettings _enemySpawnSettings;
    private PlayerMobController _playerMobController;
    private BoardCoordinateSystem _boardCoordinateSystem;
    private UserController _userController;

    [Inject]
    private void Construct
    (
       CoroutineService coroutineService
       , MobView.Factory mobFactory
        ,EnemySpawnSettings enemySpawnSettings
        ,PlayerMobController playerMobController
        ,BoardCoordinateSystem boardCoordinateSystem
        ,UserController userController
    )
    {
        _coroutineService = coroutineService;
        _mobFactory = mobFactory;
        _enemySpawnSettings = enemySpawnSettings;
        _playerMobController = playerMobController;
        _boardCoordinateSystem = boardCoordinateSystem;
        _userController = userController;
    }
    #endregion
    public MobView CreateMobView()
    {
        MobView mob = _mobFactory.Create();
        return mob;
    }

   
    protected override void OnInitialize()
    {
        _signalBus.Subscribe<LevelFailSignal>(Disable);
        _signalBus.Subscribe<LevelSuccessSignal>(Disable);
    }

    public void ManuelInit()
    {
        if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
    }

    public void StartSpawn()
    {
       
        _coroutineHandle = _coroutineService.StartCoroutine(Spawn());
    }

    private IEnumerator<float> Spawn()
    {
       

        yield return Timing.WaitForSeconds(_enemySpawnSettings.SpawnDelay);
        var lsNötr = _boardCoordinateSystem.GetNötrMilitaryBase();
        var lsPlayer = _boardCoordinateSystem.LsPlayerMilitaryBaseView;
        float randomValue = Random.value;
      
        for (int i = 0; i < _boardCoordinateSystem.lsEnemyMilitaryBaseView.Count; i++)
        {
            if(_boardCoordinateSystem.lsEnemyMilitaryBaseView[i].SoldierCount>0)
            {
                if (randomValue <= 0.3f && lsNötr.Count > 0) // %30 şans ile nötr
                {
                    int randomIndex = Random.Range(0, lsNötr.Count);
                    _boardCoordinateSystem.lsEnemyMilitaryBaseView[i].SendingTroops(lsNötr[randomIndex]);

                }
                else
                {
                    int randomIndex = Random.Range(0, lsPlayer.Count);
                    _boardCoordinateSystem.lsEnemyMilitaryBaseView[i].SendingTroops(lsPlayer[randomIndex]);

                }
            }

            yield return Timing.WaitForSeconds(_enemySpawnSettings.UnitDelay);
        }
        
        _signalBus.Fire<StartEnemySpawnTimerSignal>();
    }
   
    private void Disable()
    {
        OnStopService();
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
