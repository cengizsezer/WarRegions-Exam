using MyProject.Core.Controllers;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Models;
using MyProject.Core.Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

public class UpdateCurrencyGameTask : GameTask
{
    #region Injection

    private CurrencyController _currencyController;
    private SignalBus _signalBus;
    private CurrencyModel _currencyModel;

    [Inject]
    private void Construct(CurrencyController currencyController
        , SignalBus signalBus
        , CurrencyModel currencyModel)
    {
        _currencyController = currencyController;
        _signalBus = signalBus;
        _currencyModel = currencyModel;
    }

    #endregion

    private CurrencyData _currencyData;
    private CurrencyUpdateType _currencyUpdateType;

    public void Initialize(CurrencyData currencyData, CurrencyUpdateType currencyUpdateType)
    {
        _currencyData = currencyData;
        _currencyUpdateType = currencyUpdateType;
       
    }

    public override void StartTask(CancellationTokenSource cancellationTokenSource,
        TaskCompleteCallback taskCompleteCallback)
    {
        base.StartTask(cancellationTokenSource, taskCompleteCallback);

        switch (_currencyUpdateType)
        {
            case CurrencyUpdateType.Gain:
                CompleteTask();
                break;
            case CurrencyUpdateType.Consume:
                UpdateCurrencyWithDelay(0).Forget();
                break;
        }
    }

    private async UniTaskVoid UpdateCurrencyWithDelay(float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        switch (_currencyData.CurrencyType)
        {
            case CurrencyType.Coin:
                _signalBus.Fire<CoinUpdatedSignal>();
                break;
            case CurrencyType.Mana:
                _signalBus.Fire<ManaUpdatedSignal>();
                break;
        }

        CompleteTask();
    }
}
