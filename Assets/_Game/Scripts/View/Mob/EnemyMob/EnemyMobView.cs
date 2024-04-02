using MyProject.Core.Controllers;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using MyProject.GamePlay.Controllers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MyProject.GamePlay.Characters
{
    public class Paths
    {
        public Transform[] path;
    }
    public partial class EnemyMobView : BaseGridItemView, IPoolable<IMemoryPool>
    {
        public class Factory : PlaceholderFactory<EnemyMobView> { }
        public class Pool : MonoPoolableMemoryPool<IMemoryPool, EnemyMobView> { }

    }
    public partial class EnemyMobView : BaseGridItemView, IPoolable<IMemoryPool>
    {
       
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

        public float maxHP = 100;

        public float HP
        {
            get => _hp;

            set
            {
                _hp = value;


                if (_hp <= 0)
                {
                    _hp = 0f;
                    if (IsAlive)
                    {
                        OnDeath();
                    }
                }
            }
        }

        public bool IsAlive = true;
        public bool IsLastEnemy = false;

        [SerializeField] private Animator[] LsAnimators;
        [SerializeField] private ParticleSystem _impact;
        [SerializeField] private float _hp;
        [SerializeField] Image _hpFillImage;
        [SerializeField] Image imgHpStart, imgHpBody;
        [SerializeField] Sprite spRedStart, spRedBase, spGreenStart, spGreenBase;

        private IMemoryPool _pool;
        private BaseItemData[] _characterItemData => _itemGroupData.ItemData;
        private Animator _currentAnimator;
        private AnimStates _animState = AnimStates.Idle;

        #region Injection
        private EnemyMobController _enemyMobController;
        private DamageTextFeedbackView.Factory _damageTextFeedbackFactory;
        private CurrencyController _currencyController;
        private BoardGamePlayController _boardGameplayController;
        private TaskService _taskService;
        private CurrencySettings _currencySettings;
        private UserController _userController;

        [Inject]
        protected virtual void Construct
       (
                ItemTweenSettings itemTweenSettings
              , CharacterItemSettings itemSettings
              , BoardFXController boardFXController
              , SignalBus signalBus
            , EnemyMobController enemyMobController
            , DamageTextFeedbackView.Factory damageTextFeedbackFactory
            , CurrencyController currencyController
            , BoardGamePlayController boardGameplayController
            , TaskService taskService
            , CurrencySettings currencySettings
            , UserController userController

       )
        {
            _itemTweenSettings = itemTweenSettings;
            _characterSettings = itemSettings;
            _boardFXController = boardFXController;
            _signalBus = signalBus;
            _enemyMobController = enemyMobController;
            _damageTextFeedbackFactory = damageTextFeedbackFactory;
            _currencyController = currencyController;
            _boardGameplayController = boardGameplayController;
            _taskService = taskService;
            _currencySettings = currencySettings;
            _userController = userController;
        }

        #endregion

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            AnimState = AnimStates.Idle;
            HP = maxHP;
            _hpFillImage.fillAmount = HP;
            imgHpStart.sprite = spGreenStart;
            imgHpBody.sprite = spGreenBase;
            IsAlive = true;
            _impact.gameObject.SetActive(false);
        }
        protected override void SetView()
        {
            SetAnimator();
            RebindAnimator();
            SetViewData();
        }

        private void SetAnimator()
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

        private void RebindAnimator()
        {
            _currentAnimator.enabled = false;
            _currentAnimator.enabled = true;
            _currentAnimator.Rebind();
            _currentAnimator.Update(0f);
        }

        private void SetViewData()
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
            currentRendererData.CharacterRenderer.WeopanHand.sprite = currentData?.Weopan;
            currentRendererData.CharacterRenderer.WeopanHand.sprite = currentData?.Weopan;
        }

        public override void Despawn()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            foreach (var item in LsAnimators)
            {
                item.gameObject.SetActive(false);
            }
            DOTween.Kill(transform);
            IsLastEnemy = false;
            _pool.Despawn(this);
        }

        public void ReceiveDamage(float f)
        {
            HP -= f;
            HP = Mathf.Clamp(HP, 0, maxHP);
            _hpFillImage.fillAmount = (float)HP / maxHP;

            if (_hpFillImage.fillAmount <= .2f)
            {
                imgHpStart.sprite = spRedStart;
                imgHpBody.sprite = spRedBase;
            }
            else
            {
                imgHpStart.sprite = spGreenStart;
                imgHpBody.sprite = spGreenBase;
            }

            var infoText = _damageTextFeedbackFactory.Create();
            infoText.Init(this.transform, f, Color.red);
        }
        public void StartMovementRoutine(List<GridView> lsGridView, float time)
        {
            StartCoroutine(MovementRoutine(lsGridView, time));
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

            transform.DOPath(_path, time).SetEase(Ease.Linear).OnComplete(() => ApplyDamage(transform.position));
        }

        public void RemoveMeEnemyList()
        {
            _enemyMobController.RemoveEnemyMobs(this);
        }

        public virtual void OnDeath()
        {
            if (!IsAlive) return;

            RemoveMeEnemyList();

            _signalBus.Fire<MoveCountViewChangedSignal>();

            IsAlive = false;
            AnimState = AnimStates.Idle;
            CurrencyGainAnimationGameTask gameTask = new CurrencyGainAnimationGameTask();
            var currencyData = new CurrencyData
            {
                CurrencyType = CurrencyType.Mana,
                CurrencyValue = _currencySettings.GetCurrencyPairData(CurrencyType.Mana).ManaCostSettings.DefaultCost
            };

            bool isWin = IsLastEnemy;
            _currencyController.AddCurrency(currencyData);
            gameTask.Initialize(currencyData, transform.localPosition, Despawn);
            _taskService.AddTask(gameTask);

            if (isWin == true)
            {
                _taskService.AddTask(new LockUIGameTask());
                _userController.LevelWin();
            }
        }

        public void OnDespawned()
        {
            StopAllCoroutines();
            Dispose();
        }

        public void ApplyDamage(Vector3 tp)
        {
            if (!_boardGameplayController.IsRunning) return;
            RaycastHit[] hits = Physics.SphereCastAll(tp, 10f, Vector3.up);
            PlayerBossView target;
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.TryGetComponent<PlayerBossView>(out target))
                    {
                        target = hits[i].collider.gameObject.GetComponent<PlayerBossView>();
                        if (target)
                        {
                            if (target.IsAlive)
                            {
                                IsAlive = false;
                                _impact.gameObject.SetActive(true);
                                _enemyMobController.LsEnemyMobViews.Remove(this);
                                target.ReceiveDamage(25f);
                                Despawn();
                            }
                        }
                    }
                }
            }
        }
    }
}

