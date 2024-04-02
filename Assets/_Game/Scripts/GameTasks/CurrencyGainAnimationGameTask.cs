using MyProject.Core.Controllers;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Services;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

public class CurrencyGainAnimationGameTask : GameTask
{

    
    #region Injection

    private CurrencyController _currencyController;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(CurrencyController currencyController
    , SignalBus signalBus)
    {
        _currencyController = currencyController;
        _signalBus = signalBus;
    }

    #endregion

    private CurrencyData _currencyData;
    private Action _onCompleteAction;
    private Vector2 _startPosition;

    public void Initialize(CurrencyData currencyData, Vector2 startPosition, Action onCompleteAction = null)
    {
        _startPosition = startPosition;
        _currencyData = currencyData;
        _onCompleteAction = onCompleteAction;
      
       
    }

    public override void StartTask(CancellationTokenSource cancellationTokenSource, TaskCompleteCallback taskCompleteCallback)
    {
        switch (_currencyData.CurrencyType)
        {
            case CurrencyType.Coin:
                _signalBus.Fire(new CoinUpdatedSignal(_startPosition, true, false));
                break;
            case CurrencyType.Mana:
                _signalBus.Fire(new ManaUpdatedSignal(_startPosition, true));
                break;
           
        }
        base.StartTask(cancellationTokenSource, taskCompleteCallback);
        CompleteTask();
    }

    public override void CompleteTask(float delay = 0)
    {
        _onCompleteAction?.Invoke();
        base.CompleteTask(delay);
    }
}
