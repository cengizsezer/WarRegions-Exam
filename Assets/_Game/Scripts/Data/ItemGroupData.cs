using MyProject.Core.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Data
{
    [CreateAssetMenu(fileName = nameof(ItemGroupData), menuName = AssetMenuName.SETTINGS + nameof(ItemGroupData))]
    public class ItemGroupData : ScriptableObject
    {
        public ItemName ItemName;
        public BaseItemData[] ItemData;
    }

    [Serializable]
    public class BaseItemData
    {
        public Sprite Weopan = null;
        public Sprite ThrowingItem = null;
        public BodySortingGroup[] ItemIcons;
    }

    [Serializable]
    public class BodySortingGroup
    {
        public Body Body;
        public Bottom Bottom;
    }

    [Serializable]
    public class Body
    {
        public Sprite Head;
        public Sprite ArmF;
        public Sprite ArmB;

    }

    [Serializable]
    public class Bottom
    {
        public Sprite BottomMiddle;
        public Sprite LegF;
        public Sprite LegB;
    }
}

