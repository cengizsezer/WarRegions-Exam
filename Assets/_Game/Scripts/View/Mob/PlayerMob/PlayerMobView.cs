using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Settings;
using MyProject.GamePlay.Controllers;
using UnityEngine;
using Zenject;


namespace MyProject.GamePlay.Characters
{
    public partial class PlayerMobView : BaseGridItemView, IPoolable<IMemoryPool>
    {
        public class Factory : PlaceholderFactory<PlayerMobView> { }
        public class Pool : MonoPoolableMemoryPool<IMemoryPool, PlayerMobView> { }

    }

    public partial class PlayerMobView : BaseGridItemView, IPoolable<IMemoryPool>
    {
        public delegate void OnUpdate();
        public OnUpdate onUpdate;
        public EnemyMobView TargetView;
        public AttackBaseView AttackBaseView;
       
        public float _attackSpeed = 0;
        private float _damage;
        public float Damage
        {
            get => _damage;

            set
            {
                _damage = value;
            }
        }

        public int Level
        {
            get => _level;

            set
            {
                SetLevelText(_level);
                SetDamage(_level);
            }
        }

        public bool isAlive = true;

        public AnimStates AnimState
        {
            get => _animState;

            set
            {
                if (_animState != value)
                {
                    _animState = value;
                    if (_currentAnimator != null)
                    {
                        _currentAnimator.SetTrigger(_animState.ToString());
                    }

                }
            }
        }

        [SerializeField] private Animator[] LsAnimators;
        [SerializeField] TMPro.TextMeshProUGUI _levelText;

        private BaseWeopanVFXView _vFXInHand;
        private int _level => ItemLevel;
        private AnimStates _animState = AnimStates.Idle;
        private IMemoryPool _pool;
        private BaseItemData[] _characterItemData => _itemGroupData.ItemData;

        #region Injection

        private WeopanVFXController _weopanVFXController;
        private BoardGamePlayController _boardGameplayController;
        private EnemyMobController _enemyMobController;
        private EnemySpawnController _enemySpawnController;
        private SignalBus _signalBus;

        [Inject]
        protected virtual void Construct
        (
                 ItemTweenSettings itemTweenSettings
               , CharacterItemSettings itemSettings
               , BoardFXController boardFXController
               , SignalBus signalBus
               , BoardGamePlayController boardGameplayController
               , WeopanVFXController weopanVFXController
               , EnemyMobController enemyMobController
               ,EnemySpawnController enemySpawnController
        )
        {
            _itemTweenSettings = itemTweenSettings;
            _characterSettings = itemSettings;
            _boardFXController = boardFXController;
            _signalBus = signalBus;
            _boardGameplayController = boardGameplayController;
            _weopanVFXController = weopanVFXController;
            _enemyMobController = enemyMobController;
            _enemySpawnController = enemySpawnController;
        }
        #endregion

        public EnemyMobView GetTarget() => TargetView;
        public BaseWeopanVFXView SetWeopan(BaseWeopanVFXView view) => _vFXInHand = view;

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            _signalBus.Subscribe<LevelFailSignal>(Despawn);
            _signalBus.Subscribe<LevelSuccessSignal>(Despawn);
            Damage = _characterSettings.PlayerGamingSettings.Damage;
            _attackSpeed = _characterSettings.PlayerGamingSettings.AttackSpeed;
            StartFight();
        }

        private void SetDamage(int multiplier)
        {
            Damage *= multiplier;
        }

        private void SetLevelText(int value)
        {
            _levelText.text = value.ToString();
        }

        public override void OnSelected(GridView gridView)
        {
            _signalBus.Fire(new CharacterItemSelectedSignal(_itemGroupData, gridView.Coordinates, ItemLevel));
            base.OnSelected(gridView);
        }

        protected override void SetView()
        {
            SetAnimator();
            RebindAnimator();
            SetViewData();
            SetAttackBase();
            SetLevelText(Level);
            SetDamage(Level);
        }

        void SetAnimator()
        {
            int _id = GetID();

            for (int i = 0; i < LsAnimators.Length; i++)
            {
                if (i == _id)
                {
                    LsAnimators[i].gameObject.SetActive(i == _id);

                    _currentAnimator = LsAnimators[i];
                }
            }
        }
        void RebindAnimator()
        {
            _currentAnimator.enabled = false;
            _currentAnimator.enabled = true;
            _currentAnimator.Rebind();
            _currentAnimator.Update(0f);
        }

        void SetViewData()
        {
            if (_characterRender[GetID()].CharacterRenderer == null || _characterItemData == null) return;

            BaseItemData currentData = _characterItemData[GetID()];
            CharacterRenderData currentRendererData = _characterRender[GetID()];
            currentRendererData.CharacterRenderer.Body.Head.sprite = currentData.ItemIcons[0].Body.Head;
            currentRendererData.CharacterRenderer.Body.ArmF.sprite = currentData.ItemIcons[0].Body?.ArmF;
            currentRendererData.CharacterRenderer.Body.ArmB.sprite = currentData.ItemIcons[0]?.Body?.ArmB;
            currentRendererData.CharacterRenderer.Bottom.BottomMiddle.sprite = currentData.ItemIcons[0]?.Bottom?.BottomMiddle;
            currentRendererData.CharacterRenderer.Bottom.LegF.sprite = currentData.ItemIcons[0]?.Bottom?.LegF;
            currentRendererData.CharacterRenderer.Bottom.LegB.sprite = currentData.ItemIcons[0]?.Bottom?.LegB;


            if (currentRendererData.CharacterRenderer.WeopanHand != null)
            {
                currentRendererData.CharacterRenderer.WeopanHand.sprite = currentData?.Weopan;
            }
        }

        void SetAttackBase()
        {
            AttackBaseView = LsAnimators[GetID()].GetComponent<AttackBaseView>();
        }
        public override bool IsMaxLevel()
        {
            return ItemLevel + 1 >= _itemGroupData.ItemData.Length;
        }
        public override void Despawn()
        {
           
            if (!gameObject.activeSelf)
            {
                return;
            }
            ClosedObjects();
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(Despawn);
            _signalBus.TryUnsubscribe<LevelFailSignal>(Despawn);
            _pool.Despawn(this);
        }

        private void ClosedObjects()
        {
            foreach (var obj in LsAnimators)
            {
                obj.gameObject.SetActive(false);
            }
        }
        public void OnDespawned()
        {
           
        }

        public virtual void StartFight()
        {
            if (onUpdate != null)
            {
                return;
            }

            onUpdate = null;

            ResetAnimatorController();

            EnemyMobView enemyView = _enemyMobController.GetTargetEnemy();

            if (!_boardGameplayController.IsRunning)
            {
                StopFight();
                return;
            }

            if (enemyView == null && _boardGameplayController.IsRunning && !_enemySpawnController.IsWaveComplate)
            {
                StartFight();

                return;
            }

            if (enemyView != null && !enemyView.IsAlive)
            {
                StopFight();
                StartFight();
                return;
            }

            TargetView = enemyView;
            onUpdate = FollowTarget;
        }

        public void StopFight()
        {
            ResetAnimatorController();
            _animState = AnimStates.Idle;
            onUpdate = null;
        }

        void FollowTarget()
        {
            if (!_boardGameplayController.IsRunning || !_enemySpawnController.IsWaveComplate)
            {
                onUpdate = null;
                StopFight();
                return;
            }

            if (TargetView == null || !TargetView.IsAlive)
            {
                onUpdate = null;
                StartFight();
                return;
            }

            StopFight();

            _characterSettings.PlayerGamingSettings.AttackTimer = _characterSettings.PlayerGamingSettings.AttackInterval / 1.2f;
            onUpdate = AttackRoutine;
        }
        void AttackRoutine()
        {
            if (!_boardGameplayController.IsRunning)
            {
                StopFight();
                onUpdate = null;
                return;
            }

            if (TargetView == null)
            {
                onUpdate = null;
                StartFight();
                return;
            }

            if (!TargetView.IsAlive)
            {
                onUpdate = null;
                TargetView = null;
                StartFight();
                return;
            }

            _characterSettings.PlayerGamingSettings.AttackTimer += Time.deltaTime;

            if (_characterSettings.PlayerGamingSettings.AttackTimer >= _characterSettings.PlayerGamingSettings.AttackInterval)
            {

                _characterSettings.PlayerGamingSettings.AttackTimer = 0f;
                AttackBaseView.Cast();
            }
        }

        private void Update()
        {
            _OnUpdate();
        }

        public virtual void _OnUpdate()
        {
            if (_boardGameplayController.IsRunning)
                onUpdate?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}


