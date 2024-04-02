using MyProject.Core.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject.Core.Models
{
    public class UserModel
    {
        public UserLevelData PlayerLevelData;
        public int Level { get; private set; }

        public void UpdateLevel(int newValue)
        {
            Level = newValue;
        }
       
    }
}

