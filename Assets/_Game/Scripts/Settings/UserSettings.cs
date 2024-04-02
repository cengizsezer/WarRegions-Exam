using MyProject.Core.Data;
using System;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(UserSettings), menuName = AssetMenuName.SETTINGS + nameof(UserSettings))]
    [Serializable]
    public class UserSettings : ScriptableObject
    {
        public UserLevelData[] PlayerLevelData;
    }
}



