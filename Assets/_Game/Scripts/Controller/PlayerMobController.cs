using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Characters;
using System;
using System.Collections.Generic;
using Zenject;

namespace MyProject.GamePlay.Controllers
{
    public class PlayerMobController:IDisposable
    {
        public List<MobView> LsMobViews = new();
        

        #region Injection


        private readonly MobView.Factory _mobFactory;
        private readonly SignalBus _signalBus;

        [Inject]
        public PlayerMobController(
            MobView.Factory mobFactory
            ,SignalBus signalBus
            )
        {

            _mobFactory = mobFactory;
            _signalBus = signalBus;
        }

        #endregion

        public void Init()
        {
            _signalBus.Subscribe<LevelFailSignal>(Disable);
            _signalBus.Subscribe<LevelSuccessSignal>(Disable);
        }
       
        public MobView CreateMobView()
        {
            MobView mob = _mobFactory.Create();
            
           
            return mob;
        }

        public void Disable()
        {
            foreach (var mob in LsMobViews)
            {
                mob.Despawn();
            }
            LsMobViews.Clear();
          
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelFailSignal>(Disable);
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(Disable);
        }
    }
}

