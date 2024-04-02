using MyProject.Core.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;



namespace MyProject.Core.Data
{
    [Serializable]
    public class BoardData
    {
        public List<GridData> GridData;
    }

    [Serializable]
    public class GridData
    {
        public Vector2Int Coordinates;
        public GridState GridState;
        public ItemData ItemData;
    }

    [Serializable]
    public class ItemData
    {
        public ItemName ItemName;
        public GameplayMobType GridItemType;
        public CharMobType CharacterType;
        public int ItemLevel = 1;
    }

}



