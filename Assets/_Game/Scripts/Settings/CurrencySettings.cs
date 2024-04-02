using MyProject.Core.Data;
using MyProject.Core.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(CurrencySettings), menuName = AssetMenuName.SETTINGS + nameof(CurrencySettings))]
    public class CurrencySettings : ScriptableObject
    {
        public CurrencyVFXView CurrencyAnim;
        public List<CurrencyPairData> CurrencyPairData;

        public CurrencyPairData GetCurrencyPairData(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.Coin:
                    return CurrencyPairData[0];
                case CurrencyType.Mana:
                    return CurrencyPairData[1];
                default:
                    return CurrencyPairData[0];
            }
        }
    }

}
