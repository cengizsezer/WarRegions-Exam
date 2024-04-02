using MyProject.Core.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MyProject.GamePlay.Controllers
{
    public class BoardFXController
    {
        #region Injection

        private BoardVFXView.Factory _boardVFXFactory;

        [Inject]
        private BoardFXController(BoardVFXView.Factory boardVFXFactory)
        {
            _boardVFXFactory = boardVFXFactory;
        }

        #endregion

        public void PlayVFX(VFXArgs args)
        {
            _boardVFXFactory.Create(args);
        }
    }

    public readonly struct VFXArgs
    {
        public readonly VFXType VFXType;
        public readonly Transform Parent;
        public readonly float Time;

        public VFXArgs(VFXType vfxType, Transform parent, float time) : this()
        {
            VFXType = vfxType;
            Parent = parent;
            Time = time;
        }
    }
}

