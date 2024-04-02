using System.Linq;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(PopupSettings), menuName = AssetMenuName.SETTINGS + nameof(PopupSettings))]
    public class PopupSettings : ScriptableObject
    {
        //public Tween.Tween DefaultPopupRevealAnimation;
        public GameObject[] inGamePopups;

        public GameObject GetPopupByName(string popupName)
        {
            return inGamePopups.FirstOrDefault(popupPrefab => popupPrefab.name == popupName);
        }

    }

}

