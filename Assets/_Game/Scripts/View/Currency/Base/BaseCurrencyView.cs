using MyProject.Core.Controllers;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Models;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using DG.Tweening;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

public class BaseCurrencyView : BaseView
{
    [SerializeField] private ParticleSystemForceField particleSystemForceField;
    private DG.Tweening.Tween _tween;

    #region Injection

    protected CurrencyModel _currencyModel;
    private CoroutineService _coroutineService;
    private CurrencySettings _currencySettings;
    private ScreenController _screenController;
    private DiContainer _diContainer;
    private CurrencyVFXView.Factory _factory;
    private Camera _camera;
    private SignalBus _signalBus;
    

    [Inject]
    private void Construct(CurrencyModel currencyModel
        , CoroutineService coroutineService
        , ScreenController screenController
        , CurrencySettings currencySettings,
        DiContainer diContainer,
        CurrencyVFXView.Factory factory,
        Camera camera,
        SignalBus signalBus
        )
    {
        _currencyModel = currencyModel;
        _coroutineService = coroutineService;
        _currencySettings = currencySettings;
        _screenController = screenController;
        _diContainer = diContainer;
        _factory = factory;
        _camera = camera;
        _signalBus = signalBus;
       
    }

    #endregion

    [SerializeField] protected TextMeshProUGUI _currencyLabel;
    protected CurrencyType _currencyType;

    private long _lastAmount;

    public override void Initialize()
    {
        _signalBus.Subscribe<ParticleHitSignal>(TakeHitFromParticle);
    }

    protected void UpdateCurrencyLabel(long newAmount)
    {
        _lastAmount = newAmount;
        _currencyLabel.text = _lastAmount.FormatNumber();
       
    }
   
    protected void UpdateCurrency(long newAmount, Vector2 startPosition, bool animate = false,
        Action onCompleteCallBack = null)
    {
        if (animate)
        {
            var dif = 0;
            if (_currencyType==CurrencyType.Coin)
            {
                 dif = Mathf.Min((int)(newAmount - _lastAmount), 10);
            }
            else
            {
                dif = 1;
            }
           

            _coroutineService.StopCoroutineWithTag(CoroutineConsts.CURRENCY_ADD_ANIM + _currencyType);
            _coroutineService.StartCoroutine(
                UpdateCurrencyLabelWithAnimation(newAmount, onCompleteCallBack),
                CoroutineConsts.CURRENCY_ADD_ANIM + _currencyType);

            _factory.Create(new CurrencyVFXView.Args(startPosition, _currencyType, dif, 1f));
        }
        else
        {
            _coroutineService.StopCoroutineWithTag(CoroutineConsts.CURRENCY_ADD_ANIM + _currencyType);
            UpdateCurrencyLabel(newAmount);
        }
    }

    private void TakeHitFromParticle(ParticleHitSignal signal)
    {
        _tween?.Kill();

        transform.localScale = Vector3.one;

        if (signal.ParticleSystemForceField != particleSystemForceField.gameObject) return;

        _tween = transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.1f).OnComplete(() =>
        {
            transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
        });
    }

    private IEnumerator<float> UpdateCurrencyLabelWithAnimation(long newAmount, Action onCompleteCallBack)
    {
        yield return Timing.WaitForSeconds(0.8f);
        var formattedAmount = newAmount;
        long animatedCoinAmount = 20;
        double amountPerCount = (formattedAmount - _lastAmount) / (float)animatedCoinAmount;
        float durationPerCoin = 1 / (animatedCoinAmount * 4);
        var dif = formattedAmount - _lastAmount;

        if ((long)amountPerCount < 1)
        {
            for (int i = 0; i < dif; i++)
            {
                UpdateCurrencyLabel(_lastAmount + 1);
                yield return Timing.WaitForSeconds(durationPerCoin);
            }
        }
        else
        {
            for (int i = 0; i < animatedCoinAmount; i++)
            {
                UpdateCurrencyLabel(_lastAmount + (long)amountPerCount);
                yield return Timing.WaitForSeconds(durationPerCoin);
            }
        }

        UpdateCurrencyLabel(newAmount);
        onCompleteCallBack?.Invoke();
    }

    public override void Dispose()
    {
        _tween?.Kill();
        _coroutineService.StopCoroutineWithTag(CoroutineConsts.CURRENCY_ADD_ANIM + _currencyType);
        _coroutineService.StopCoroutineWithTag(CoroutineConsts.CURRENCY_FLY_ANIM + _currencyType);
        _signalBus.Unsubscribe<ParticleHitSignal>(TakeHitFromParticle);
    }
}
