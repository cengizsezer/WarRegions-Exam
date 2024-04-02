using MyProject.Core.Enums;
using System;
using UnityEngine;


namespace MyProject.Core.Data
{
    [Serializable]
    public struct CurrencyData
    {
        public CurrencyType CurrencyType;
        public long CurrencyValue;

        public CurrencyData(CurrencyType currencyType, long currencyValue)
        {
            CurrencyType = currencyType;
            CurrencyValue = currencyValue;
        }
    }

    [Serializable]
    public struct CurrencySaveData
    {
        public long DefaultCoinAmount;
        public long DefaultManaAmount;
     
    }

    [Serializable]
    public struct CurrencyPairData
    {
        public CurrencyType CurrencyType;
        public Sprite Sprite;
        public ManaCostSettings ManaCostSettings;
    }
    [Serializable]
    public struct ManaCostSettings
    {
        public int DefaultCost;
        public int PerEnemyKillingMana;
        public int DefaultManaValue;
    }
   

}


