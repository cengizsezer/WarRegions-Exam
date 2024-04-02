using MyProject.Core.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CoinCurrencyView : BaseCurrencyView
{
    public Image CoinCurrencyIcon;
    #region Injection

    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    #endregion

    public override void Initialize()
    {
        _currencyType = CurrencyType.Coin;
        _signalBus.Subscribe<CoinUpdatedSignal>(OnCoinUpdated);

        UpdateCurrencyLabel(_currencyModel.CoinValue);
        base.Initialize();
    }


    private void OnCoinUpdated(CoinUpdatedSignal signal)
    {
        UpdateCurrency(_currencyModel.CoinValue, signal.StartPosition, signal.Animate, OnAnimationComplete);
    }

    private void OnAnimationComplete()
    {
    }

    public override void Dispose()
    {
        _signalBus.Unsubscribe<CoinUpdatedSignal>(OnCoinUpdated);
    }
}
