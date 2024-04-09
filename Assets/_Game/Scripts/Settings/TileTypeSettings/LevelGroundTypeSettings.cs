using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject.Core.Settings
{
    [System.Serializable]
    public class LevelGroundTypeSettings
    {
        [SerializeField] private uint width = 4;
        [SerializeField] private uint height = 4;
        public GridStartLayout gridStartLayout = new GridStartLayout(9, 9);

        public float xOffset, zOffset;

        public uint Width => width;
        public uint Height => height;

    }

}
