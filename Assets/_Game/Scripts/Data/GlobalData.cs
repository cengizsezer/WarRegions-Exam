using MyProject.Core.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;



namespace MyProject.Core.Data
{

    [Serializable]
    public class SoldierWarData
    {
        public Color color;
        public BlockType MobBlockType;
        public Vector3 SpawnPosition;
        public MillitaryBaseView TargetMilitaryBase;
        public MillitaryBaseView OwnerMilitaryBase;
        public int SoldierCount;
        public List<GridView> Path;
       
    }

}



