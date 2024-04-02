using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject.Core.Data
{
    [Serializable]
    public class CharacterRenderData
    {
        public SortingGroupRenderer CharacterRenderer;
    }

    [Serializable]
    public class SortingGroupRenderer
    {
        public SpriteRenderer WeopanHand;
        public BodyRenderer Body;
        public BottomRenderer Bottom;
    }

    [Serializable]
    public class BodyRenderer
    {
        public SpriteRenderer Head;
        public SpriteRenderer ArmF;
        public SpriteRenderer ArmB;

    }

    [Serializable]
    public class BottomRenderer
    {
        public SpriteRenderer BottomMiddle;
        public SpriteRenderer LegF;
        public SpriteRenderer LegB;
    }

}
