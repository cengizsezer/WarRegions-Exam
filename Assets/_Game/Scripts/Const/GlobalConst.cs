using UnityEngine;

namespace MyProject.Core.Const
{
    public class GlobalConsts
    {
        public const string GameName = "TowerDefence";
        private const string Prefix = "MyProject_" + GameName + "_";
        private const string FlagPrefix = "MyProject_" + GameName + "_Flag_";
        public class AssetMenuName
        {
            public const string ParentMenuName = "MyProject/";
            public const string INSTALLERS = ParentMenuName + "Installers/";
            public const string SETTINGS = ParentMenuName + "Settings/";

        }
        public  class CoroutineConsts
        {
            public const string CUSTOMER_SPAWN_TAG = "customer_spawn_tag";
            public const string CUSTOMER_MOVE_TAG = "customer_move_tag";

            public const string CURRENCY_ADD_ANIM = "currency_add_anim";
            public const string CURRENCY_FLY_ANIM = "currency_fly_anim";

            public const string BONUSITEMVIEWCLOSEANIM = "bonus_item_view_close_anim";
            public const string HAPTIC_WITH_COUNT_TAG = "haptic_with_count_tag";
            public const string HAPTIC_WITH_TIME_TAG = "haptic_with_time_tag";

            public const string TIMER_TAG = "timer";
        }
        public struct PopupName
        {
            public const string AdminPanelPopup = "AdminPanelPopup";
            public const string LevelUpPopup = "LevelUpPopup";
            public const string LevelFailPopup = "LevelFailPopup";
            public const string ShopPopup = "ShopPopup";
        }
        public struct Animations
        {
            public const string Disappear = "Disappear";
        }
        public struct Flags
        {
            
            public static readonly string PurchaseFlag = FlagPrefix + "PurchaseManaFlag";
            public static readonly string ShopFlag = FlagPrefix + "ShopFlag";
            public static readonly string BoardFlag = FlagPrefix + "BoardFlag";
           
        }
        public struct SaveData
        {
            // Currency Data
            public static readonly string COINS_KEY = "COIN";
            public static readonly string GEMS_KEY = "GEM";
            public static readonly string ENERGY_KEY = "ENERGY";
            public static readonly string PLAYER_LEVEL = "LEVEL";
            public static readonly string PLAYER_XP = "XP";
            public static readonly string REGENERATION_KEY = "LastRegenerationTime";
            public static readonly string IAP_PURCHASE_KEY = "IAP_PURCHASE";
            public static readonly string TUTORIAL_STEP = "TUTORIAL_STEP";
            public static readonly string TUTORIAL_PROGRESS_INDEX = "TUTORIAL_PROGRESS_INDEX";


        }
        public struct LayerMasks
        {
            public static readonly int UI = LayerMask.GetMask("UI");
            public static readonly int COLLIDER = LayerMask.GetMask("Collider");
        }
        public struct SortingOrders
        {
            public static readonly int GridViewDefault = 120;
            public static readonly int CharacterDefault = 150;
            public static readonly int CharacterSelect = 200;
        }
        public struct BoardConsts
        {
            public const int ROWS = 5;
            public const int COLUMNS = 7;
            public const int ENEMY_SPAWN_COOLDOWN = 3;
            public const int ENEMY_SPAWN_COUNT = 5;
            public static bool AreCoordinatesInsideTheBoard(Vector2Int coordinates)
            {
                return coordinates.x >= 0 &&
                       coordinates.x < ROWS &&
                       coordinates.y >= 0 &&
                       coordinates.y < COLUMNS;
            }
        }
    }
}

