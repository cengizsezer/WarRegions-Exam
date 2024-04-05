using MyProject.Core.Const;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Settings;
using MyProject.GamePlay.Controllers;
using DG.Tweening;
using MyProject.Tween;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;


namespace MyProject.GamePlay.Characters
{
    public abstract class BaseGridItemView : BaseView
    {
       
        public MilitaryBaseType MilitaryBaseType { get; private set; }
        public Animator _currentAnimator;
        [SerializeField] protected DOTweener _doTweener;
        private DG.Tweening.Tween _moveTween;
        private DG.Tweening.Tween _scaleTween;

        #region Injection

        protected ItemTweenSettings _itemTweenSettings;
        protected MobGamePlaySettings _characterSettings;
        protected BoardFXController _boardFXController;
        protected SignalBus _signalBus;

        [Inject]
        protected virtual void Construct
        (
            ItemTweenSettings itemTweenSettings
            , MobGamePlaySettings itemSettings
            , BoardFXController boardFXController
            , SignalBus signalBus)
        {
            _itemTweenSettings = itemTweenSettings;
            _characterSettings = itemSettings;
            _boardFXController = boardFXController;
            _signalBus = signalBus;
        }

        #endregion


        public int GetID() => (int)MilitaryBaseType;
      
        public override void Initialize()
        {
            SetView();
        }
        public void Init(ItemData itemData)
        {
            Initialize();
        }
        protected void ResetAnimatorController()
        {
            if (_currentAnimator == null) return;

            foreach (var parametre in _currentAnimator.parameters)
            {
                if (parametre.type == AnimatorControllerParameterType.Trigger)
                {
                    _currentAnimator.ResetTrigger(parametre.name);
                }
            }
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
        protected abstract void SetView();
        public abstract void Despawn();
    }
}


