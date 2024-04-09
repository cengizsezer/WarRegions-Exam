using MyProject.Core.Data;
using MyProject.Core.Enums;
using System;
using Zenject;

namespace MyProject.Core.Models
{
    public class CurrencyModel : IInitializable, IDisposable
    {
        public long CoinValue { get; private set; }

        public void Initialize()
        {
            CoinValue=SaveLoadController.GetCoin();
        }

        public void UpdateCoin(long newValue)
        {
            CoinValue = newValue;
        }
       

        public bool HasEnoughCurrency(CurrencyData currencyData)
        {
            switch (currencyData.CurrencyType)
            {
                case CurrencyType.Coin:
                    return currencyData.CurrencyValue <= CoinValue;
                default:
                    return false;
            }
        }

        public void Dispose()
        {
        }
    }
}


