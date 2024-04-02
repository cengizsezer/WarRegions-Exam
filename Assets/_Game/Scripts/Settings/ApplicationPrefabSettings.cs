using MyProject.Core.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(ApplicationPrefabSettings), menuName = AssetMenuName.SETTINGS + nameof(ApplicationPrefabSettings))]
    public class ApplicationPrefabSettings : ScriptableObject
    {
        public BuildType buildType;
        public bool runInBackground;

        [FoldoutGroup("MainView")] public GameObject MainScreenPrefab;
        [FoldoutGroup("MainView")] public GameObject MapScreenPrefab;
        [FoldoutGroup("GridView")] public GameObject GridViewPrefab;
        [FoldoutGroup("VFX")] public GameObject BoardVFXView;
        [FoldoutGroup("VFX")] public GameObject BombWeopanVFXViewPrefab;
        [FoldoutGroup("VFX")] public GameObject AxeWeopanVFXViewPrefab;
        [FoldoutGroup("VFX")] public GameObject ArrowWeopanVFXViewPrefab;
        [FoldoutGroup("FeedBacks")] public GameObject DamageTextPrefab;
        [FoldoutGroup("Boss")] public GameObject PlayerBossPrefab;
        [FoldoutGroup("Boss")] public GameObject EnemyBossPrefab;


    }

}


