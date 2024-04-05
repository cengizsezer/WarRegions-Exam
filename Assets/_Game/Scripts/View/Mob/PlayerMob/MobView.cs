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
    public class Paths
    {
        public Transform[] path;
    }
    public partial class MobView : BaseGridItemView, IPoolable<IMemoryPool>
    {
        public class Factory : PlaceholderFactory<MobView> { }
        public class Pool : MonoPoolableMemoryPool<IMemoryPool, MobView> { }

    }

    public partial class MobView : BaseGridItemView, IPoolable<IMemoryPool>, IMessageReceiver
    {
        protected Damageable Damageable;
        public MobView TargetView;
        public MillitaryBaseView TargetMilitaryBaseView;
        public int Amount;

        public delegate void OnUpdate();
        public OnUpdate onUpdate;

        public bool IsAlive;
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
      

        public bool isAlive = false;
        private Animator _currentAnimator;
        private SkinnedMeshRenderer _skinnedMeshRenderer;

        public SkinnedMeshRenderer GetSmr() => _skinnedMeshRenderer;
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
        [SerializeField] private SkinnedMeshRenderer[] LsRenderer;
        [SerializeField] private GameObject[] goParent;
        private AnimStates _animState = AnimStates.Idle;
        private IMemoryPool _pool;
        private int _soldierCount;
        

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

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            isAlive = true;
            _signalBus.Subscribe<LevelFailSignal>(Despawn);
            _signalBus.Subscribe<LevelSuccessSignal>(Despawn);
            Damageable = GetComponent<Damageable>();
            Damageable.onDamageMessageReceivers.Add(this);
            Damage = _characterSettings.PlayerGamingSettings.Damage;
            _attackSpeed = _characterSettings.PlayerGamingSettings.AttackSpeed;
            StartFight();
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
            base.Initialize();
            SetAnimator();
            RebindAnimator();
            SetViewData();
            SetAttackBase();
        }
        public void SetPropsView(SoldierWarData data)
        {
            transform.position = data.SpawnPosition;
            _skinnedMeshRenderer.material.color = data.color;
            TargetMilitaryBaseView = data.TargetMilitaryBase;
            _soldierCount = data.SoldierCount;
            
        }
        private void Attack()
        {
            Damageable d = GetTarget().Damageable;

            if (d != null)
            {

                Damageable.DamageMessage message = new Damageable.DamageMessage
                {
                    damager = this,
                    amount = Amount,
                };

                d.ApplyDamage(message);
            }
        }
        private void Death(Damageable.DamageMessage msg)
        {

        }


        private void ApplyDamage(Damageable.DamageMessage msg)
        {

        }

        public void StartMovementRoutine(List<GridView> lsGridView, float time)
        {
            StartCoroutine(MovementRoutine(lsGridView, time));
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
        private IEnumerator MovementRoutine(List<GridView> lsGridView, float time)
        {
            ResetAnimatorController();
            AnimState = AnimStates.Run;
            Paths paths = new Paths();
            paths.path = new Transform[lsGridView.Count];

            for (int i = 0; i < lsGridView.Count; i++)
            {
                paths.path[i] = lsGridView[i].transform;

            }

            yield return transform.DOMove(paths.path[0].position, 0.1f).WaitForCompletion();

            Vector3[] _path = new Vector3[paths.path.Length];

            for (int i = 0; i < paths.path.Length; i++)
            {
                _path[i] = paths.path[i].position;
            }

            transform.DOPath(_path, time).SetEase(Ease.Linear).OnComplete(() => Debug.Log("sona geldim"));
        }

        private MobView SearchForEnemy()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 10f, Vector3.up);
            MobView target = null;
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.TryGetComponent<MobView>(out target))
                    {
                        target = hits[i].collider.gameObject.GetComponent<MobView>();
                        if (target)
                        {
                          
                        }
                    }
                }

            }

            return target;


        }

      

        void SetAnimator()
        {
            int _id = GetID();

            for (int i = 0; i < LsAnimators.Length; i++)
            {
                if (i == _id)
                {
                    goParent[i].SetActive(i == _id);

                    _currentAnimator = LsAnimators[i];
                    _skinnedMeshRenderer = LsRenderer[i];
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
           
        }

        void SetAttackBase()
        {
            //AttackBaseView = LsAnimators[GetID()].GetComponent<AttackBaseView>();
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

            MobView enemyView = SearchForEnemy();

            //if (!_boardGameplayController.IsRunning)
            //{
            //    StopFight();
            //    return;
            //}

            //if (enemyView == null && _boardGameplayController.IsRunning && !_enemySpawnController.IsWaveComplate)
            //{
            //    StartFight();

            //    return;
            //}

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
            //if (!_boardGameplayController.IsRunning || !_enemySpawnController.IsWaveComplate)
            //{
            //    onUpdate = null;
            //    StopFight();
            //    return;
            //}

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
            //if (!_boardGameplayController.IsRunning)
            //{
            //    StopFight();
            //    onUpdate = null;
            //    return;
            //}

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
                //AttackBaseView.Cast();
            }
        }

        private void Update()
        {
            _OnUpdate();
        }

        public virtual void _OnUpdate()
        {
            //if (_boardGameplayController.IsRunning)
            //    onUpdate?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

       
    }
}


