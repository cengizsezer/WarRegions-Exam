using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Models;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using System;
using UnityEngine;
using Zenject;

namespace MyProject.Core.Controllers
{
    public class CurrencyController : BaseController
    {
        public long CurrentCostManaValue { get; set; }
       
        #region Injection

        private CurrencyModel _currencyModel;
        private TimerService _timerService;
        private TaskService _taskService;
        private SignalBus _signalBus;
        private CurrencySettings _currencySettings;

        private ScreenController _screenController;
        [Inject]
        private void Construct(CurrencyModel currencyModel
            , TimerService timerService
            , TaskService taskService
            , SignalBus signalBus
            ,CurrencySettings currencySettings
            ,ScreenController screenController
            )
        {
            _currencyModel = currencyModel;
            _timerService = timerService;
            _taskService = taskService;
            _signalBus = signalBus;
            _currencySettings = currencySettings;
            _screenController = screenController;
        }

        #endregion

        public void Init()
        {
            CurrencyData currencyData = new CurrencyData
            {
                CurrencyType = CurrencyType.Mana,
                CurrencyValue = 100
            };

            AddCurrency(currencyData);
            CurrentCostManaValue = _currencySettings.GetCurrencyPairData(CurrencyType.Mana).ManaCostSettings.DefaultCost;
        }
       
        public long GetCurrentManaCost()
        {
            return CurrentCostManaValue;
        }

        public void IncreaseCurrentManaCost()
        {
            CurrentCostManaValue = CurrentCostManaValue + _currencySettings.GetCurrencyPairData(CurrencyType.Mana).ManaCostSettings.PerEnemyKillingMana;
        }
        public void AddCurrency(CurrencyData currencyData)
        {
            var newValue = GetCurrency(currencyData.CurrencyType) + currencyData.CurrencyValue;
            UpdateCurrency(new CurrencyData(currencyData.CurrencyType, newValue));
        }
        public void ResetCurrency(CurrencyType currencyType)
        {
            CurrentCostManaValue = _currencySettings.GetCurrencyPairData(CurrencyType.Mana).ManaCostSettings.DefaultCost;
            UpdateCurrencyGameTask gameTask = new UpdateCurrencyGameTask();

            CurrencyData currencyData;

            switch (currencyType)
            {
                case CurrencyType.Coin:
                    currencyData = new CurrencyData { CurrencyType = CurrencyType.Coin, CurrencyValue = _currencyModel.CoinValue };
                    break;
                case CurrencyType.Mana:
                    currencyData = new CurrencyData
                    { 
                         CurrencyType = CurrencyType.Mana
                        ,CurrencyValue = _currencySettings.GetCurrencyPairData(currencyType).ManaCostSettings.DefaultManaValue 
                    };
                    break;
                default:
                    currencyData = new CurrencyData();
                    break;
            }

            UpdateCurrency(new CurrencyData(currencyData.CurrencyType, currencyData.CurrencyValue));
            gameTask.Initialize(currencyData, CurrencyUpdateType.Consume);
            _taskService.AddTask(gameTask);
        }
        public long GetCurrency(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.Coin:
                    return _currencyModel.CoinValue;
                case CurrencyType.Mana:
                    return _currencyModel.ManaValue;
            }

            return 0;
        }

        public bool TryConsume(CurrencyData currencyData)
        {

            if (!_currencyModel.HasEnoughCurrency(currencyData)) return false;
            var newValue = GetCurrency(currencyData.CurrencyType) - currencyData.CurrencyValue;
            UpdateCurrency(new CurrencyData(currencyData.CurrencyType, newValue));
           
            return true;
        }

        public void UpdateCurrency(CurrencyData currencyData)
        {
           
            switch (currencyData.CurrencyType)
            {
                case CurrencyType.Coin:
                    _currencyModel.UpdateCoin(currencyData.CurrencyValue);
                    break;
                case CurrencyType.Mana:
                    _currencyModel.UpdateMana(currencyData.CurrencyValue);
                    break;
            }
        }

        private void OnCurrencyGain(CurrencyGainSignal signal)
        {
            CurrencyGainAnimationGameTask gameTask = new CurrencyGainAnimationGameTask();
            gameTask.Initialize(GetCurrencyData(signal.ItemName, signal.Count), signal.Position);
            _taskService.AddTask(gameTask);
        }

        protected override void OnInitialize()
        {
            _signalBus.Subscribe<CurrencyGainSignal>(OnCurrencyGain);
        }

        protected override void OnApplicationReadyToStart()
        {
        }

        protected override void OnDispose()
        {
            _signalBus.Unsubscribe<CurrencyGainSignal>(OnCurrencyGain);
        }

        public CurrencyData GetCurrencyData(ItemName itemName, int count)
        {
            switch (itemName)
            {
                case ItemName.Coin:
                    return new CurrencyData(CurrencyType.Coin, count);
                case ItemName.Mana:
                    return new CurrencyData(CurrencyType.Mana, count);
                default:
                    return new CurrencyData(CurrencyType.Coin, count);
            }
        }
    }
}

