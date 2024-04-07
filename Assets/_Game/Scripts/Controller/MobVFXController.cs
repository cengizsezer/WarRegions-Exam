using MyProject.Core.Enums;
using UnityEngine;
using Zenject;


namespace MyProject.GamePlay.Controllers
{
    public class MobVFXController
    {
        #region Injection

        private MobVFXView.Factory _mobVFXFactory;

        [Inject]
        private MobVFXController(MobVFXView.Factory mobVFXfactory)
        {
            _mobVFXFactory = mobVFXfactory;
        }

        #endregion

        public void PlayVFX(VFXArgs args)
        {
            _mobVFXFactory.Create(args);
        }
    }

    public  struct VFXArgs
    {
        public  VFXType VFXType;
        public  Transform Parent;
        public  float Time;

        public VFXArgs(VFXType vfxType, Transform parent, float time) : this()
        {
            VFXType = vfxType;
            Parent = parent;
            Time = time;
        }
    }
}

