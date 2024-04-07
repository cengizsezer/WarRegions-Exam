using DG.Tweening;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Settings;
using MyProject.GamePlay.Controllers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;


namespace MyProject.GamePlay.Characters
{
    public partial class MobView : BaseGridItemView, IPoolable<IMemoryPool>, IMessageReceiver
    {
        public class Factory : PlaceholderFactory<MobView> { }
        public class Pool : MonoPoolableMemoryPool<IMemoryPool, MobView> { }

        [SerializeField]private ResourceTypeData _resourceTypeData;

        public ResourceTypeData ResourceTypeData
        {
            get => _resourceTypeData;
            set
            {
                _resourceTypeData = value;
                SetResourceDataProps();
            }
        }
        public Damageable Damageable;
        public MobView TargetView;
        public MilitaryBaseView TargetMilitaryBaseView;
        public MilitaryBaseView OwnerMilitaryBaseView;
      
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
        
        private AnimStates _animState = AnimStates.Idle;
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

        [SerializeField] TextMeshPro _soldierCountText;
        [SerializeField] private Animator[] LsAnimators;
        [SerializeField] private SkinnedMeshRenderer[] LsRenderer;
        [SerializeField] private GameObject[] goParent;
        
        private IMemoryPool _pool;
        private int _soldierCount;
        public int SoldierCount
        {
            get => _soldierCount;

            set
            {
                _soldierCount = value;

                _soldierCountText.text = _soldierCount.ToString();

                if(_soldierCount<=0)
                {
                    IsAlive = false;
                }

            }
        }
       
        public ColorType MobColorType;
        #region Injection
        
        private SignalBus _signalBus;
        private MobVFXController _mobVfxController;

        [Inject]
        protected virtual void Construct
        (
              SignalBus signalBus,
              MobVFXController mobVFXController
        )
        {
            _signalBus = signalBus;
            _mobVfxController = mobVFXController;
        }
        #endregion

        public SkinnedMeshRenderer GetSmr() => _skinnedMeshRenderer;
        public MobView GetTarget() => TargetView;
        public int GetSoldierCount() => SoldierCount;

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

        public void PlayParticle(VFXType vfxType)
        {
            _mobVfxController.PlayVFX(new VFXArgs(vfxType, transform, 2f));
        }
        public override void Initialize()
        {
            StartMovementRoutine(_currentPath,1f);
        }

        private List<GridView> _currentPath = new();
        public void SetPropsView(SoldierWarData data)
        {
            MilitaryBaseType = data.MilitaryBaseType;
            _animState = AnimStates.Idle;

            SetAnimator();
            RebindAnimator();
            SetAttackBase();
            ResetAnimatorController();

            transform.position = data.SpawnPosition + Vector3.up * 3f;
            TargetMilitaryBaseView = data.TargetMilitaryBase;
            SoldierCount = data.SoldierCount;
            _currentPath = data.Path;
            OwnerMilitaryBaseView = data.OwnerMilitaryBase;
            TargetMilitaryBaseView = data.TargetMilitaryBase;
            ResourceTypeData = data.ResourceTypeData;
          
        }

        private void SetResourceDataProps()
        {
            MobColorType = ResourceTypeData.ColorType;
            _skinnedMeshRenderer.material.color = ToColorFromHex(ResourceTypeData.HexColor);
        }
       
        private void Death(Damageable.DamageMessage msg)
        {
            Despawn();
        }

        private void ApplyDamage(Damageable.DamageMessage msg)
        {
            SoldierCount=Damageable.SoldierCount;
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
        private IEnumerator MovementAndAttackRoutine(List<GridView> lsGridView, float moveTime, float attackInterval)
        {
            ResetAnimatorController();
            AnimState = AnimStates.Run;

            int startIndex = 0; // Başlangıç indexi

            while (startIndex < lsGridView.Count)
            {
                GridView targetGrid = lsGridView[startIndex];
                Vector3 targetPosition = targetGrid.transform.position + Vector3.up * 3f;

                // Düşmanı ara
                TargetView = SearchForEnemy();

                if (TargetView)
                {
                    // Düşman bulundu, hareketi iptal et ve ateş et
                    AnimState = AnimStates.Idle;
                    Damageable.SoldierCount = SoldierCount;
                    _currentAttackBase.Cast();
                    yield return new WaitForSeconds(attackInterval);
                    AnimState = AnimStates.Run; // Animasyonu tekrar koşuya geçir

                    // Hedef öldüğünde TargetView'i null olarak ayarla ve yeni bir hedef ara
                    TargetView = null;
                    continue; // Hedef ölmediği için bir sonraki adıma geçme
                }
                else
                {
                    // Düşman bulunamadı, hedefe doğru hareket et
                    transform.DOLookAt(targetPosition, .2f).WaitForCompletion();
                    yield return transform.DOMove(targetPosition, moveTime).SetEase(Ease.Linear).WaitForCompletion();
                }

                startIndex++; // Index'i bir sonraki hedefe taşı
            }

            TargetMilitaryBaseView.TakeOver(this);
            Despawn();
        }

        //private IEnumerator MovementAndAttackRoutine(List<GridView> lsGridView, float moveTime, float attackInterval)
        //{
        //    ResetAnimatorController();
        //    AnimState = AnimStates.Run;

        //    int startIndex = 0; // Başlangıç indexi

        //    while (startIndex < lsGridView.Count)
        //    {
        //        GridView targetGrid = lsGridView[startIndex];
        //        Vector3 targetPosition = targetGrid.transform.position + Vector3.up * 3f;

        //        transform.DOLookAt(targetPosition, .2f).WaitForCompletion();
        //        yield return transform.DOMove(targetPosition, moveTime).SetEase(Ease.Linear).WaitForCompletion();

        //        // Eğer hedef grid üzerinde düşman varsa, ateş et
        //        TargetView = null;
        //        TargetView = SearchForEnemy();

        //        if (TargetView)
        //        {
        //            AnimState = AnimStates.Idle;
        //            Damageable.SoldierCount = SoldierCount;

        //            while (TargetView.IsAlive)
        //            {
        //                _currentAttackBase.Cast();

        //                yield return new WaitForSeconds(attackInterval);
        //            }

        //            // Hedef öldüğünde TargetView'i null olarak ayarla ve yeni bir hedef ara
        //            TargetView = null;
        //            AnimState = AnimStates.Run;
        //        }
        //        else
        //        {
        //            // Etrafı tarayıp, düşman bulmaya çalış
        //            MobView newTarget = SearchForEnemy();
        //            if (newTarget)
        //            {
        //                // Yeni hedef bulundu, hedefe yönel ve ateş et
        //                TargetView = newTarget;
        //                AnimState = AnimStates.Idle;
        //                Damageable.SoldierCount = SoldierCount;

        //                while (TargetView.IsAlive)
        //                {
        //                    _currentAttackBase.Cast();

        //                    yield return new WaitForSeconds(attackInterval);
        //                }

        //                // Hedef öldüğünde TargetView'i null olarak ayarla ve yeni bir hedef ara
        //                TargetView = null;
        //                AnimState = AnimStates.Run;
        //            }
        //            else
        //            {
        //                // Hiç düşman yok, yoluna devam et
        //                AnimState = AnimStates.Run;
        //            }
        //        }

        //        startIndex++; // Index'i bir sonraki hedefe taşı
        //    }

        //    TargetMilitaryBaseView.TakeOver(this);
        //    Despawn();
        //}

        //private IEnumerator MovementAndAttackRoutine(List<GridView> lsGridView, float moveTime, float attackInterval)
        //{
        //    ResetAnimatorController();
        //    AnimState = AnimStates.Run;

        //    int startIndex = 0; // Başlangıç indexi

        //    while (startIndex < lsGridView.Count)
        //    {
        //        GridView targetGrid = lsGridView[startIndex];
        //        Vector3 targetPosition = targetGrid.transform.position + Vector3.up * 3f;

        //        transform.DOLookAt(targetPosition, .2f).WaitForCompletion();
        //        yield return transform.DOMove(targetPosition, moveTime).SetEase(Ease.Linear).WaitForCompletion();

        //        // Eğer hedef grid üzerinde düşman varsa, ateş et
        //        TargetView = null;
        //        TargetView = SearchForEnemy();
               
        //        if (TargetView)
        //        {
        //            AnimState = AnimStates.Idle;
        //            Damageable.SoldierCount = SoldierCount;

        //            while (TargetView.IsAlive)
        //            {
        //                _currentAttackBase.Cast();

        //                yield return new WaitForSeconds(attackInterval);
        //            }

        //            // Hedef öldüğünde TargetView'i null olarak ayarla ve yeni bir hedef ara
        //            TargetView = null;
        //            startIndex++;
        //            AnimState = AnimStates.Run;
        //            continue;
        //        }

               
        //        AnimState = AnimStates.Run;
        //        startIndex++; // Index'i bir sonraki hedefe taşı
        //    }

        //    TargetMilitaryBaseView.TakeOver(this);
        //    Despawn();
        //}

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
                        if (target && target.ResourceTypeData.ColorType!= MobColorType && target!=this &&target.IsAlive)
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
           
            for (int i = 0; i < goParent.Length; i++)
            {
                goParent[i].SetActive(false);

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

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            IsAlive = true;
            Damageable = GetComponent<Damageable>();
            Damageable.onDamageMessageReceivers.Add(this);
            Damage = _characterSettings.PlayerGamingSettings.Damage;
            _attackSpeed = _characterSettings.PlayerGamingSettings.AttackSpeed;
        }

        public override void Despawn()
        {

            if (!gameObject.activeSelf)
            {
                return;
            }
            IsAlive = false;
            TargetView = null;
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


