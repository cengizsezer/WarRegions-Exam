using MyProject.Core.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;



namespace MyProject.Core.Data
{

    [Serializable]
    public class SoldierWarData
    {
        public MilitaryBaseType MilitaryBaseType;
        public ResourceTypeData ResourceTypeData;
        public Vector3 SpawnPosition;
        public MilitaryBaseView TargetMilitaryBase;
        public MilitaryBaseView OwnerMilitaryBase;
        public int SoldierCount;
        public List<GridView> Path;
       
    }

}



