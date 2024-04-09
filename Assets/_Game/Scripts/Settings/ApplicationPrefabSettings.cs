using MyProject.Core.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(ApplicationPrefabSettings), menuName = AssetMenuName.SETTINGS + nameof(ApplicationPrefabSettings))]
    public class ApplicationPrefabSettings : ScriptableObject
    {
        public GameObject MainScreenPrefab;
        public GameObject MapScreenPrefab;
        public GameObject GridViewPrefab;
        public GameObject MilitaryBaseViewPrefab;
        public GameObject MobVFXView;
        public GameObject DamageTextPrefab;

    }

}


