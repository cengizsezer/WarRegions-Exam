using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(LevelSettings), menuName = AssetMenuName.SETTINGS + nameof(LevelSettings))]
    public class LevelSettings : ScriptableObject
    {
        public List<LevelData> LevelDataList;
      
    }

   
}
