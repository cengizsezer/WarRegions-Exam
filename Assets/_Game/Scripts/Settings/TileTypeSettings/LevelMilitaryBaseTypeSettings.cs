using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject.Core.Settings
{
    [System.Serializable]
    public class LevelMilitaryBaseTypeSettings
    {
        public GridStartLayout gridStartLayout = new GridStartLayout(9, 9);
        public float[] WaitingTimes;
        public int[] DefaultCounts;
        public int[] MaxSoldierCounts;
        public int SoldierIncreaseValue;

    }

}
