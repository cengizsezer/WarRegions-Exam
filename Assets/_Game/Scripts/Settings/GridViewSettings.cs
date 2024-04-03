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
        public GridViewData GridViewData;
       
    }

    [Serializable]
    public class GridViewData
    {
        public int xSize, zSize;
        public float xOffset, zOffset;
    }
}

