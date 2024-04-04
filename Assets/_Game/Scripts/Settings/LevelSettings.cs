using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(LevelSettings), menuName = AssetMenuName.SETTINGS + nameof(LevelSettings))]
    public class LevelSettings : ScriptableObject
    {
        public List<LevelData> LevelData;
        [SerializeField] private LevelGroundTypeSettings levelGroundTypeSettings;
        [SerializeField] private LevelMilitaryBaseTypeSettings militaryBaseTypeSettings;
        [SerializeField] private LevelMountainSettings levelMountainSettings;


        public LevelGroundTypeSettings LevelGroundTypeSettings => levelGroundTypeSettings;
        public LevelMilitaryBaseTypeSettings MilitaryBaseTypeSettings => militaryBaseTypeSettings;
        public LevelMountainSettings LevelMountainSettings => levelMountainSettings;
    }

   
}
