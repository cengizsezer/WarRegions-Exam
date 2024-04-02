using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(GridViewSettings), menuName = AssetMenuName.SETTINGS + nameof(GridViewSettings))]
    public class GridViewSettings : ScriptableObject
    {
        public RoadViewData RoadViewData;
        public Sprite CharacterFieldSprite;
    }

    [Serializable]
    public class RoadViewData
    {
        public Sprite RightSprite;
        public Sprite LeftSprite;
        public Sprite HorizontalSprite;
        public Sprite VerticalSprite;
    }
}

