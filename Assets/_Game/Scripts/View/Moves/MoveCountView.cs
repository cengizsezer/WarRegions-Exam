using MyProject.Core.Settings;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class MoveCountView : BaseView
{
    [SerializeField] protected TextMeshProUGUI _movesLabel;
    private int _currentKillingEnemyCount = 0;
    #region Injection

    private EnemySpawnSettings _enemySpawnSettings;
    private SignalBus _signalBus;
    [Inject]
    private void Construct(EnemySpawnSettings enemySpawnSettings,SignalBus signalBus)
    {
        _enemySpawnSettings = enemySpawnSettings;
        _signalBus = signalBus;
    }

    #endregion
    public override void Initialize()
    {
        _signalBus.Subscribe<MoveCountViewChangedSignal>(ChangeCount);
        _movesLabel.text = (_enemySpawnSettings.MaxSpawnWaveCount * _enemySpawnSettings.SingleWaveEnemyCount) + " " + "/" + " " + _currentKillingEnemyCount;
    }

    private void ChangeCount()
    {
        _currentKillingEnemyCount++;
        _movesLabel.text = (_enemySpawnSettings.MaxSpawnWaveCount * _enemySpawnSettings.SingleWaveEnemyCount) + " " + "/" + " " + _currentKillingEnemyCount;
    }

    public override void Dispose()
    {
        _signalBus.Unsubscribe<MoveCountViewChangedSignal>(ChangeCount);
    }
}
