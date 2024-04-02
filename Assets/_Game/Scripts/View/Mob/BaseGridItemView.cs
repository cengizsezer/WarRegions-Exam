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
    public abstract class BaseGridItemView : BaseView, IGridItem
    {
        public GameplayMobType GridItemType { get; private set; }
        public CharMobType CharacterType { get; private set; }
        public ItemName ItemName { get; private set; }
        public int ItemLevel { get; set; }
        public Animator _currentAnimator;

        [SerializeField] protected DOTweener _doTweener;
        [SerializeField] protected SortingGroup[] _sortings;
        [SerializeField] protected CharacterRenderData[] _characterRender;

        protected ItemGroupData _itemGroupData;
        public ItemGroupData ItemGroupData => _itemGroupData;

        private DG.Tweening.Tween _moveTween;
        private DG.Tweening.Tween _scaleTween;

        #region Injection

        protected ItemTweenSettings _itemTweenSettings;
        protected CharacterItemSettings _characterSettings;
        protected BoardFXController _boardFXController;
        protected SignalBus _signalBus;

        [Inject]
        protected virtual void Construct
        (
            ItemTweenSettings itemTweenSettings
            , CharacterItemSettings itemSettings
            , BoardFXController boardFXController
            , SignalBus signalBus)
        {
            _itemTweenSettings = itemTweenSettings;
            _characterSettings = itemSettings;
            _boardFXController = boardFXController;
            _signalBus = signalBus;
        }

        #endregion

        public int GetID() => (int)CharacterType;
        public override void Initialize()
        {
            _itemGroupData = _characterSettings.GetItemGroupData(ItemName);
            SetView();
        }
        public void Init(ItemData itemData)
        {
            GridItemType = itemData.GridItemType;
            CharacterType = itemData.CharacterType;
            ItemLevel = itemData.ItemLevel;
            ItemName = itemData.ItemName;
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
            gridView.ChangeState(GridState.Filled);
            transform.SetParent(gridView.ItemHolder);
            _sortings[GetID()].sortingOrder = GlobalConsts.SortingOrders.CharacterDefault;
            transform.localRotation = Quaternion.identity;
            transform.localScale = _characterSettings.characterLocalScale;
            transform.localPosition = new Vector3(0, 0, -1);
            _doTweener.PlayNamed(Tweens.SPAWN1);
            _doTweener.PlayNamed(Tweens.SPAWN2);
        }

        public virtual void OnSelected(GridView gridView)
        {
            _moveTween?.Kill();
            _scaleTween?.Kill();
            _sortings[GetID()].sortingOrder = GlobalConsts.SortingOrders.CharacterSelect;
        }
        public void OnDragged(Vector3 dragPosition)
        {
            transform.position = Vector3.Lerp(transform.position, dragPosition,
                Time.deltaTime * _itemTweenSettings.DragSpeed);
        }
        public virtual void OnMerged(GridView gridView)
        {
            BondWithGrid(gridView);
            ItemLevel++;
            SetView();
            OnSelected(gridView);
        }

        public void MoveToGrid(GridView gridView, float speed)
        {
            _moveTween?.Kill();
            _scaleTween?.Kill();
            _moveTween = transform.DOMove(gridView.ItemHolder.position, speed)
                .SetEase(_itemTweenSettings.MoveEase).OnComplete(() => { BondWithGrid(gridView); });
        }

        public override void Dispose()
        {
            ItemLevel = 1;
            _moveTween?.Kill();
            _scaleTween?.Kill();
            _doTweener.KillAll();
        }

        public virtual bool IsMaxLevel()
        {
            return default;
        }

        protected abstract void SetView();
        public abstract void Despawn();
    }
}


