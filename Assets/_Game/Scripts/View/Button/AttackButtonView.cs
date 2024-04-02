using MyProject.Core.Controllers;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Models;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

public class AttackButtonView : BaseButtonView
{
    [SerializeField] private Transform shakeObject;
    public  ButtonSetterView _buttonViewSetter;
    public Transform ShakeTransform => shakeObject;

    private DG.Tweening.Tween _tween;


    private CurrencyModel _currencyModel;
    private CurrencyController _currencyController;
    [Inject]
    private void Construct(

        CurrencyModel currencyModel
        ,CurrencyController currencyController

      )
    {
        _currencyController = currencyController;
        _currencyModel = currencyModel;
       
    }
    private void Start()
    {
        base.Initialize();
       
    }

    public override void Initialize()
    {
        _signalBus.Subscribe<ManaUpdatedSignal>(SetButtonView);
    }

    public override void Dispose()
    {
        _signalBus.Unsubscribe<ManaUpdatedSignal>(SetButtonView);
    }

    public void SetButtonView()
    {
        var cost = _currencyController.GetCurrentManaCost();
        var currencyData = new CurrencyData { CurrencyType = CurrencyType.Mana, CurrencyValue = cost };

        _buttonViewSetter.SetText(cost.FormatNumber());
       
        if (_currencyModel.HasEnoughCurrency(currencyData))
        {
            _buttonViewSetter.ButtonActivate();
        }
        else
        {
            _buttonViewSetter.ButtonDeactivate();
        }
    }
    protected override void OnButtonClicked()
    {
        if (!_flagService.IsFlagAvailable(Flags.BoardFlag)) return;
        _flagService.SetFlag(Flags.BoardFlag, FlagState.Unavailable);

        _signalBus.Fire<AttackButtonClickSignal>();
        _flagService.SetFlag(Flags.BoardFlag, FlagState.Available);
    }

    public void TakeHit()
    {
        _tween?.Kill();

        shakeObject.localScale = Vector3.one;

        _tween = shakeObject.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).OnComplete(() => { shakeObject.DOScale(new Vector3(1f, 1f, 1f), 0.1f); });
    }
}


