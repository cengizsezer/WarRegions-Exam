using MyProject.Core.Data;
using MyProject.Core.Enums;
using System;
using Zenject;

namespace MyProject.Core.Models
{
    public class CurrencyModel : IInitializable, IDisposable
    {
        public long CoinValue { get; private set; }
        public long ManaValue { get; private set; }

        public void Initialize()
        {
        }

        public void UpdateCoin(long newValue)
        {
            CoinValue = newValue;
        }

        public void UpdateMana(long newValue)
        {
            ManaValue = newValue;
        }

        public bool HasEnoughCurrency(CurrencyData currencyData)
        {
            switch (currencyData.CurrencyType)
            {
                case CurrencyType.Coin:
                    return currencyData.CurrencyValue <= CoinValue;
                case CurrencyType.Mana:
                    return currencyData.CurrencyValue <= ManaValue;
                default:
                    return false;
            }
        }

        public void Dispose()
        {
        }
    }
}


