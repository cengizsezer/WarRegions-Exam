using DG.Tweening;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Settings;
using MyProject.GamePlay.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MyProject.GamePlay.Characters
{
    public partial class MobView : BaseGridItemView, IPoolable<IMemoryPool>
    {
        public class Factory : PlaceholderFactory<MobView> { }
        public class Pool : MonoPoolableMemoryPool<IMemoryPool, MobView> { }

    }

    public partial class MobView : BaseGridItemView, IPoolable<IMemoryPool>, IMessageReceiver
    {
        public Damageable Damageable;
        public MobView TargetView;
        public MillitaryBaseView TargetMilitaryBaseView;
        public MillitaryBaseView OwnerMilitaryBaseView;
      
        public bool IsAlive = false;
        public float _attackSpeed = 0;
        private int _damage;
        public int Damage
        {
            get => _damage;

            set
            {
                _damage = value;
            }
        }
       
        public Animator CurrentAnimator;
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private AttackBase _currentAttackBase;
        public SkinnedMeshRenderer GetSmr() => _skinnedMeshRenderer;
        public AnimStates AnimState
        {
            get => _animState;

            set
            {
                if (_animState != value)
                {
                    _animState = value;
                    if (CurrentAnimator != null)
                    {
                        CurrentAnimator.SetTrigger(_animState.ToString());
                    }

                }
            }
        }

        [SerializeField] private Animator[] LsAnimators;
        [SerializeField] private SkinnedMeshRenderer[] LsRenderer;
        [SerializeField] private GameObject[] goParent;
        private AnimStates _animState = AnimStates.Idle;
        private IMemoryPool _pool;
        private int _soldierCount;
        private GridView _startGridView;
        private GridView _endGridView;
        public BlockType MobBlockType;
        #region Injection

        private MobVFXController _weopanVFXController;
        private SignalBus _signalBus;

        [Inject]
        protected virtual void Construct
        (
                 ItemTweenSettings itemTweenSettings
               , MobGamePlaySettings itemSettings
               , BoardFXController boardFXController
               , SignalBus signalBus
               , MobVFXController weopanVFXController
             
        )
        {
            _itemTweenSettings = itemTweenSettings;
            _characterSettings = itemSettings;
            _boardFXController = boardFXController;
            _signalBus = signalBus;
            _weopanVFXController = weopanVFXController;
           
        }
        #endregion

        public MobView GetTarget() => TargetView;
        public int GetSoldierCount() => _soldierCount;
        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            IsAlive = true;
            _signalBus.Subscribe<LevelFailSignal>(Despawn);
            _signalBus.Subscribe<LevelSuccessSignal>(Despawn);
            Damageable = GetComponent<Damageable>();
            Damageable.onDamageMessageReceivers.Add(this);
            Damage = _characterSettings.PlayerGamingSettings.Damage;
            _attackSpeed = _characterSettings.PlayerGamingSettings.AttackSpeed;
        }

        public void OnReceiveMessage(MobMessageType type, object sender, object msg)
        {
            switch (type)
            {
                case MobMessageType.DAMAGED:
                    {
                        Damageable.DamageMessage message = (Damageable.DamageMessage)msg;
                        ApplyDamage(message);
                    }
                    break;
                case MobMessageType.DEAD:
                    {
                        Damageable.DamageMessage message = (Damageable.DamageMessage)msg;
                        Death(message);
                    }
                    break;
            }
        }

        public override void Initialize()
        {
            StartMovementRoutine(_currentPath,1f);
        }

        private List<GridView> _currentPath = new();
        public void SetPropsView(SoldierWarData data)
        {
            SetAnimator();
            RebindAnimator();
            SetAttackBase();

            transform.position = data.SpawnPosition+Vector3.up*3f;
            _skinnedMeshRenderer.material.color = data.color;
            TargetMilitaryBaseView = data.TargetMilitaryBase;
            _soldierCount = data.SoldierCount;
            _currentPath = data.Path;
        }
       
        private void Death(Damageable.DamageMessage msg)
        {
            Despawn();
        }

        private void ApplyDamage(Damageable.DamageMessage msg)
        {
            _soldierCount -= msg.Damage;
        }

        public void StartMovementRoutine(List<GridView> lsGridView, float time)
        {
            StartCoroutine(MovementAndAttackRoutine(lsGridView, time,3f));
        }

        protected void ResetAnimatorController()
        {
            if (CurrentAnimator == null) return;

            foreach (var parametre in CurrentAnimator.parameters)
            {
                if (parametre.type == AnimatorControllerParameterType.Trigger)
                {
                    CurrentAnimator.ResetTrigger(parametre.name);
                }
            }
        }
        IEnumerator MovementAndAttackRoutine(List<GridView> lsGridView, float moveTime, float attackInterval)
        {
            ResetAnimatorController();
            AnimState = AnimStates.Run;
           
            for (int i = 0; i < lsGridView.Count; i++)
            {
                GridView targetGrid = lsGridView[i];
                Vector3 targetPosition = targetGrid.transform.position+Vector3.up*3f;
               
                yield return transform.DOLookAt(targetPosition,.2f).WaitForCompletion();
                yield return transform.DOMove(targetPosition, moveTime).WaitForCompletion();

                // Eğer hedef grid üzerinde düşman varsa, ateş et
                TargetView = SearchForEnemy();
                if (TargetView)
                {
                    AnimState = AnimStates.Idle;
                    Damageable.currentHitPoints = TargetView._soldierCount;
                    while (TargetView.IsAlive)
                    {
                        _currentAttackBase.Cast();
                        Debug.Log(TargetView.name, TargetView.gameObject);
                        yield return new WaitForSeconds(attackInterval);

                        if(!TargetView.IsAlive)
                        {
                            TargetView = null;
                            TargetView = SearchForEnemy();

                            if (TargetView == null)
                            {
                                yield break;
                            }
                        }
                       
                    }
                   
                }

                AnimState = AnimStates.Run;
            }

            Debug.Log("buraya girmiyor mu?");
            TargetMilitaryBaseView.TakeOver(_soldierCount,this);
        }
     

        private MobView SearchForEnemy()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3f, Vector3.up);
            MobView target = null;
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.TryGetComponent<MobView>(out target))
                    {
                        target = hits[i].collider.gameObject.GetComponent<MobView>();
                        if (target && target.MobBlockType!=BlockType.Blue&&target!=this)
                        {
                            return target;
                        }
                    }
                }

            }


            return null;

        }
        void SetAnimator()
        {
            int _id = GetID();

            for (int i = 0; i < LsAnimators.Length; i++)
            {
                if (i == _id)
                {
                    goParent[i].SetActive(i == _id);

                    CurrentAnimator = LsAnimators[i];
                    _skinnedMeshRenderer = LsRenderer[i];
                }
            }
        }
        void RebindAnimator()
        {
            CurrentAnimator.enabled = false;
            CurrentAnimator.enabled = true;
            CurrentAnimator.Rebind();
            CurrentAnimator.Update(0f);
        }
        void SetAttackBase()
        {
            _currentAttackBase = LsAnimators[GetID()].GetComponent<AttackBase>();
            _currentAttackBase.parentMob = this;
        }
      
        public override void Despawn()
        {
           
            if (!gameObject.activeSelf)
            {
                return;
            }
           
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(Despawn);
            _signalBus.TryUnsubscribe<LevelFailSignal>(Despawn);
            _pool.Despawn(this);
        }
      
        public void OnDespawned()
        {
           
        }

        public override void Dispose()
        {
            base.Dispose();
        }

       
    }
   
}


