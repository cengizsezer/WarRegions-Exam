using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MyProject.GamePlay.Controllers
{
    public class WeopanVFXController
    {
        #region Injection

        private readonly BombWeopanVFXView.Factory _bombWeopanVFXFactory;
        private readonly AxeWeopanVFXView.Factory _axeWeopanVFXFactory;
        private readonly ArrowWeopanVFXView.Factory _arrowWeopanVFXFactory;

        [Inject]
        public WeopanVFXController
            (
            BombWeopanVFXView.Factory bombWeopanVFXFactory
            , AxeWeopanVFXView.Factory axeWeopanVFXFactory
            , ArrowWeopanVFXView.Factory arrowWeopanVFXFactory
            )
        {

            _bombWeopanVFXFactory = bombWeopanVFXFactory;
            _axeWeopanVFXFactory = axeWeopanVFXFactory;
            _arrowWeopanVFXFactory = arrowWeopanVFXFactory;
        }

        #endregion

        public BaseWeopanVFXView GetWeopanElement(AttackType attackType)
        {
            return attackType switch
            {
                AttackType.Bow => _arrowWeopanVFXFactory.Create(),
                AttackType.Bomb => _bombWeopanVFXFactory.Create(),
                AttackType.Axe => _axeWeopanVFXFactory.Create(),
                _ => null
            };
        }
    }
}

