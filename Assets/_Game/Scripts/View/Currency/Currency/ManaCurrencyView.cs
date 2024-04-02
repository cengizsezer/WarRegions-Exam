using MyProject.Core.Controllers;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ManaCurrencyView : BaseCurrencyView
{
    public Image manaCurrencyIcon;
    #region Injection

    private SignalBus _signalBus;
    private ScreenController _screenController;
    [Inject]
    private void Construct(SignalBus signalBus, ScreenController screenController)
    {
        _signalBus = signalBus;
        _screenController = screenController;
    }

    #endregion

    public override void Initialize()
    {
        _currencyType = CurrencyType.Mana;
        _signalBus.Subscribe<ManaUpdatedSignal>(OnManaUpdated);
        UpdateCurrencyLabel(_currencyModel.ManaValue);
        _screenController.SetCost();
        base.Initialize();
    }

    public void ManaReset()
    {
        CurrencyData currencyData = new CurrencyData
        {
            CurrencyType = CurrencyType.Mana,
            CurrencyValue = 100
        };
    }

    private void OnManaUpdated(ManaUpdatedSignal signal)
    {
        UpdateCurrency(_currencyModel.ManaValue, signal.StartPosition, signal.Animate, OnAnimationComplete);
        _screenController.SetCost();
    }

    private void OnAnimationComplete()
    {
    }

    public override void Dispose()
    {
        _signalBus.Unsubscribe<ManaUpdatedSignal>(OnManaUpdated);
    }
}
