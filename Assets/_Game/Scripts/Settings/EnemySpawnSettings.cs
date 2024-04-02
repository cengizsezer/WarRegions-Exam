using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(EnemySpawnSettings), menuName = AssetMenuName.SETTINGS + nameof(EnemySpawnSettings))]
    public class EnemySpawnSettings : ScriptableObject
    {
        public float WaveDelay;
        public float UnitSpawnDelay;
        public int MaxSpawnWaveCount;
        public int SingleWaveEnemyCount;
    }

}


