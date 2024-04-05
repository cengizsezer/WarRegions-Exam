using MyProject.Core.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MyProject.GamePlay.Controllers
{
    public class MobVFXController
    {
        #region Injection

        private readonly SeaSoldierVFXView.Factory _seaSoldierVFXViewFactory;
        private readonly LandSoldierVFXView.Factory _landSoldierVFXViewFactory;

        [Inject]
        public MobVFXController
            (
            SeaSoldierVFXView.Factory seaSoldierVFXViewFactory
            , LandSoldierVFXView.Factory landSoldierVFXViewFactory

            )
        {

            _seaSoldierVFXViewFactory = seaSoldierVFXViewFactory;
            _landSoldierVFXViewFactory = landSoldierVFXViewFactory;
           
        }

        #endregion

        public BaseMobVFXView GetWeopanElement(MilitaryBaseType attackType)
        {
            return attackType switch
            {
                MilitaryBaseType.Sea => _seaSoldierVFXViewFactory.Create(),
                MilitaryBaseType.Land => _landSoldierVFXViewFactory.Create(),
                _ => null
            };
        }
    }
}

