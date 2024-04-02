using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Characters;
using System;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterItemSettings), menuName = AssetMenuName.SETTINGS + nameof(CharacterItemSettings))]
    public class CharacterItemSettings : ScriptableObject
    {
        public PlayerMobView PlayerMobViewPrefab;
        public EnemyMobView EnemyMobViewPrefab;
        [Space(20)]
        public int characterGridItemPoolSize;
        public string characterGridItemPoolName;
        public Vector3 characterLocalScale;
        [Space(20)]
        public CharacterItemsData CharacterItems;
        public PlayerGamingSettings PlayerGamingSettings;
        public EnemyGamingSettings EnemyGamingSettings;

        public ItemGroupData GetItemGroupData(ItemName itemName)
        {
            return itemName switch
            {
                ItemName.Player => CharacterItems.PlayerGroupData,
                ItemName.Enemy => CharacterItems.EnemyGroupData,
                _ => null
            };
        }
    }

    [Serializable]
    public class CharacterItemsData
    {
        public ItemGroupData PlayerGroupData;
        public ItemGroupData EnemyGroupData;
    }

    [Serializable]
    public class PlayerGamingSettings
    {
        public float Damage;
        public float AttackSpeed;
        public float AttackInterval;
        public float AttackTimer;
        public float PlayerBossHP;
    }

    [Serializable]
    public class EnemyGamingSettings
    {
        public float MovementSpeed;
    }
}




