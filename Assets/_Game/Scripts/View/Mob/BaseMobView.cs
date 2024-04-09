using MyProject.Core.Const;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Settings;
using DG.Tweening;
using MyProject.Tween;
using UnityEngine;
using Zenject;


namespace MyProject.GamePlay.Views
{
    public abstract class BaseMobView : BaseView
    {
       
        public MilitaryBaseType MilitaryBaseType { get; set; }
       
        [SerializeField] protected DOTweener _doTweener;
        private DG.Tweening.Tween _moveTween;
        private DG.Tweening.Tween _scaleTween;

        #region Injection

        protected ItemTweenSettings _itemTweenSettings;
        protected MobGamePlaySettings _characterSettings;
        protected SignalBus _signalBus;

        [Inject]
        protected virtual void Construct
        (
            ItemTweenSettings itemTweenSettings
            , MobGamePlaySettings itemSettings
           
            , SignalBus signalBus)
        {
            _itemTweenSettings = itemTweenSettings;
            _characterSettings = itemSettings;
            _signalBus = signalBus;
        }

        #endregion


        public int GetID() => (int)MilitaryBaseType;

        public override void Initialize()
        {
            
        }
       
       

        public void BondWithGrid(GridView gridView)
        {
            _moveTween?.Kill();
            _scaleTween?.Kill();
            transform.localRotation = Quaternion.identity;
            transform.localScale = _characterSettings.characterLocalScale;
            transform.localPosition = new Vector3(0, 0, -1);
            _doTweener.PlayNamed(Tweens.SPAWN1);
            _doTweener.PlayNamed(Tweens.SPAWN2);
        }

        public override void Dispose()
        {
            _moveTween?.Kill();
            _scaleTween?.Kill();
            _doTweener.KillAll();
        }
        
        public abstract void Despawn();
    }
}


