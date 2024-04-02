using Sirenix.OdinInspector;
using UnityEngine;

namespace MyProject.General
{

    [System.Serializable]
    public struct SelectiveVector3
    {
        public ToType ToType;
        [HorizontalGroup("Group 1", LabelWidth = 20)]
        [DisableIf("HasToX")]
        public float x;
        [HorizontalGroup("Group 1", LabelWidth = 20)]
        [DisableIf("HasToY")]
        public float y;
        [HorizontalGroup("Group 1", LabelWidth = 20)]
        [DisableIf("HasToZ")]
        public float z;

        public bool HasToX { get => ToType.HasFlag(ToType.ToX); }
        public bool HasToY { get => ToType.HasFlag(ToType.ToY); }
        public bool HasToZ { get => ToType.HasFlag(ToType.ToZ); }

        public Vector3 GetValue(Vector3 defaultVector)
        {
            float newX = ToType.HasFlag(ToType.ToX) ? defaultVector.x : x;
            float newY = ToType.HasFlag(ToType.ToY) ? defaultVector.y : y;
            float newZ = ToType.HasFlag(ToType.ToZ) ? defaultVector.z : z;
            return new Vector3(newX, newY, newZ);
        }

    }

}
