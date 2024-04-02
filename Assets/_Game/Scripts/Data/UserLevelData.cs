using Sirenix.OdinInspector;
using System;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Data
{
    [CreateAssetMenu(fileName = nameof(UserLevelData), menuName = AssetMenuName.SETTINGS + nameof(UserLevelData))]
    [Serializable]
    public class UserLevelData : ScriptableObject
    {
        public int CompletionRequiredXP;
        [InfoBox("Limited creators get their usage counts from their settings.", InfoMessageType.Info)]
        public CurrencyData[] RewardCurrencyData;

    }

}

