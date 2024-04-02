using Sirenix.OdinInspector;
using UnityEngine;

namespace MyProject.General
{

    [System.Serializable]
    public struct SelectiveVector2
    {
        public ToType ToType;
        [HorizontalGroup("Group 1", LabelWidth = 20)]
        [DisableIf("HasToX")]
        public float x;
        [HorizontalGroup("Group 1", LabelWidth = 20)]
        [DisableIf("HasToY")]
        public float y;
        public bool HasToX { get => ToType.HasFlag(ToType.ToX); }
        public bool HasToY { get => ToType.HasFlag(ToType.ToY); }

        public Vector2 GetValue(Vector2 defaultVector)
        {
            float newX = ToType.HasFlag(ToType.ToX) ? defaultVector.x : x;
            float newY = ToType.HasFlag(ToType.ToY) ? defaultVector.y : y;
            return new Vector2(newX, newY);
        }

    }

}
