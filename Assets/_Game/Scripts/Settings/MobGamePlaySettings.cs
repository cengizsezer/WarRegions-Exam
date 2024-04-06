using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Characters;
using System;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(MobGamePlaySettings), menuName = AssetMenuName.SETTINGS + nameof(MobGamePlaySettings))]
    public class MobGamePlaySettings : ScriptableObject
    {
        public MobView MobViewPrefab;
       
        [Space(20)]
        public int characterGridItemPoolSize;
        public string characterGridItemPoolName;
        public Vector3 characterLocalScale;
        [Space(20)]
        public MobAttackSettings PlayerGamingSettings;
        public MobMovementSettings EnemyGamingSettings;
       
    }

    [Serializable]
    public class MobAttackSettings
    {
        public int Damage;
        public float AttackSpeed;
    }

    [Serializable]
    public class MobMovementSettings
    {
        public float MovementSpeed;
    }
}




