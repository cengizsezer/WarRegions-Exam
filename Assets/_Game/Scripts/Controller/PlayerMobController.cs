using MyProject.Core.Controllers;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Characters;
using System;
using System.Collections.Generic;
using Zenject;

namespace MyProject.GamePlay.Controllers
{
    public class PlayerMobController:BaseController
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
        protected override void OnInitialize()
        {
            _signalBus.Subscribe<LevelFailSignal>(Disable);
            _signalBus.Subscribe<LevelSuccessSignal>(Disable);
        }

        protected override void OnApplicationReadyToStart()
        {
            
        }

        protected override void OnDispose()
        {
            _signalBus.TryUnsubscribe<LevelFailSignal>(Disable);
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(Disable);
        }
    }
}

