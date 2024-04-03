using DG.Tweening;
using MyProject.Core.Const;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.GamePlay.Characters;
using MyProject.GamePlay.Controllers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
public enum HexTypes
{
    None,
    Addition,
    Substract,
    Multiply,
    Divide,
    Health,
    Merge,
    Upgrade,
    Tent,
    Speed,
    Empty
}

public enum InitialMobType
{
    None = -1,
    Player = 0,
    Enemy = 1
}

public  class GridView : BaseView, IPoolable<GridView.Args, IMemoryPool>
{
    [Header("VALUES AND TEXT")]
    [HideInInspector]
    internal int value;
    [HideInInspector]
    internal HexTypes mType;

    [Header("INITIAL VALUES")]
    [SerializeField] bool hasInitialOwner;
    
    [SerializeField] InitialMobType initialMob;
    //[SerializeField] EnemyTypes enemyID;


    [Header("PROPS")]
    [SerializeField] SkinnedMeshRenderer smr;
    [SerializeField] TextMeshPro txt;
    [SerializeField] ParticleSystem psConvert;
    public GameObject[] borders;
    [SerializeField] GameObject goUpgrade, goSpeed;
    [SerializeField] SpriteRenderer tentSprite;
    [SerializeField] MeshRenderer tentRenderer;
    [SerializeField] Collider colTrigger;

    //[HideInInspector]
    public int x, z;

    delegate void OnCollected();
    OnCollected onCollected = null;

    bool hasCollectedBefore = false;

    [HideInInspector]
    //public MobBase Owner
    //{
    //    get => _owner;
    //    set
    //    {
    //        if (_owner != value)
    //        {

    //            if (_owner != null)
    //            {
    //                _owner.DecreaseHexaCount();

    //                if (mType == HexTypes.Tent)
    //                    StopTentCooldown();
    //            }

    //            _owner = value;

    //            if (_owner != null)
    //            {
    //                _owner.IncreaseHexaCount();

    //                OnConverted();

    //                if (!hasCollectedBefore)
    //                {
    //                    onCollected?.Invoke();
    //                    hasCollectedBefore = true;
    //                }
    //                else if (mType == HexTypes.Tent)
    //                {
    //                    onCollected?.Invoke();
    //                    if (tentSpawnTween != null)
    //                        tentSpawnTween.Kill(false);
    //                    StartTentCooldown();
    //                }

    //                if (!_owner.IsPlayer())
    //                {
    //                    _owner.RemoveHexFromList(this);
    //                }
    //                else
    //                {
    //                    //Vibrator.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
    //                    //SoundManager.I.PlayOnce(Sounds.HexConquer);
    //                }

    //            }

    //            SetColorOfOwner();
    //        }
    //    }
    //}

    float _timerConvertion = 0f;

    private void Update()
    {
        //HandleConvertTime();

        //if (GameManager.isRunning)
        //{
        //    CheckWaitingList();
        //}

    }

    void CheckWaitingList()
    {
        //if (lsWaitingList.Count > 0)
        //{
        //    for (int i = 0; i < lsWaitingList.Count; i++)
        //    {
        //        if (AmINeighbourOfOwner(lsWaitingList[i]))
        //        {
        //            SetPlayerInside(lsWaitingList[i]);
        //        }
        //    }
        //}
    }

    //public void Init(bool hasInitial, InitialMobType imt = InitialMobType.None, EnemyTypes et = EnemyTypes.Red)
    //{
    //    hasInitialOwner = hasInitial;
    //    initialMob = imt;

    //    if (hasInitialOwner)
    //    {
    //        //if (initialMob == InitialMobType.Player)
    //        //{
    //        //    Owner = PlayerController.I.playerMob;
    //        //    PlayerController.I.playerMob.transform.position = transform.position.WithY(PlayerController.I.playerMob.transform.position.y);
    //        //}
    //        //else if (initialMob == InitialMobType.Enemy)
    //        //{
    //        //    enemyID = et;
    //        //    Owner = LevelHandler.I.GetLevel().enemies[(int)enemyID];
    //        //    Owner.gameObject.SetActive(true);
    //        //}

    //        if (Owner != null)
    //        {
    //            SetPlayerInside(Owner);
    //            firstInsideMob = Owner;
    //            Owner.SetInitialPosition(transform.position);
    //        }
    //    }
    //    else
    //    {
    //        Owner = null;
    //        SetColorOfOwner();
    //        SetProps();
    //    }
    //}


    #region INIT PROPS
    void SetProps()
    {
        txt.gameObject.SetActive(true);
        if (mType == HexTypes.None)
            NoneProps();
        else if (mType == HexTypes.Addition)
            SumProps();
        else if (mType == HexTypes.Substract)
            SubstractProps();
        else if (mType == HexTypes.Multiply)
            MultiplyProps();
        else if (mType == HexTypes.Upgrade)
            LevelUpProps();
        else if (mType == HexTypes.Empty)
            EmptyProps();
        else if (mType == HexTypes.Tent)
            TentProps();
        else if (mType == HexTypes.Speed)
            SpeedProp();
    }

    void SpeedProp()
    {
        txt.gameObject.SetActive(false);
        goSpeed.SetActive(true);
        onCollected = OnSpeedCollected;
    }

    void OnSpeedCollected()
    {
        goSpeed.SetActive(false);
        //Owner.SpeedUp();
        onCollected = null;
    }

    void TentProps()
    {
        txt.gameObject.SetActive(false);
        tentRenderer.gameObject.SetActive(true);
        tentSprite.gameObject.SetActive(false);
        onCollected = OnTentCollected;
    }

    void OnTentCollected()
    {
        float sclae = 1.832535f;
        tentSprite.gameObject.SetActive(true);
        //tentRenderer.materials[1].DOColor((Owner.IsPlayer() ? Color.red : Color.blue, .3f));
        tentRenderer.transform.DOScale(sclae * 1.12f, .18f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            tentRenderer.transform.DOScale(sclae, .2f).SetEase(Ease.InBounce);
        });
        StartTentCooldown();
    }

    void NoneProps()
    {
        txt.gameObject.SetActive(false);
        onCollected = null;
    }

    Tween tentSpawnTween = null;

    private void StartTentCooldown()
    {
        //if (!GameManager.isRunning) return;

        //if (tentSpawnTween != null)
        //{
        //    tentSpawnTween.Kill(false);
        //    tentSpawnTween = null;
        //}
        //float fillAmount = 360f;
        //tentSprite.material.SetFloat("_Arc2", fillAmount);

        //tentSpawnTween = DOTween.To(() => fillAmount, _x => fillAmount = _x, 0, 5f).OnUpdate(() =>
        //{
        //    if (GameManager.isRunning)
        //        tentSprite.material.SetFloat("_Arc2", fillAmount);
        //}).OnComplete(() =>
        //{
        //    if (GameManager.isRunning)
        //    {
        //        smr.SetBlendShapeWeight(1, 0);
        //        tentSpawnTween = null;
        //        StartTentCooldown();
        //        SoldierSpawner.I.SpawnSoldier(Owner, 1, tentRenderer.transform);
        //    }

        //}).SetEase(Ease.Linear);


    }

    private void StopTentCooldown()
    {
        if (tentSpawnTween != null)
        {
            tentSpawnTween.Kill(false);
            tentSpawnTween = null;
        }
        tentSprite.material.SetFloat("_Arc2", 0f);
    }

    //SUM
    void SumProps()
    {
        txt.gameObject.SetActive(true);
        txt.text = "+" + value.ToString();
        txt.color = new Color(124f / 255f, 1f, 123f / 255f, 200f / 255f);
        onCollected = SumCollected;
    }
    void SumCollected()
    {
        txt.gameObject.SetActive(false);
        //SoldierSpawner.I.SpawnSoldier(Owner, value);
    }
    //SUBSTRACT
    void SubstractProps()
    {
        txt.text = "-" + value.ToString();
        txt.color = new Color(1f, 38f / 255f, 38f / 255f, 200f / 255f);
        onCollected = SubstractCollected;
    }
    void SubstractCollected()
    {
        txt.gameObject.SetActive(false);
        //SoldierSpawner.I.KillSoldiers(Owner, value);
    }
    //MULTIPLY
    void MultiplyProps()
    {
        txt.text = "x" + value.ToString();
        txt.color = new Color(124f / 255f, 1f, 123f / 255f, 200f / 255f);
        onCollected = MultiplyCollected;
    }
    void MultiplyCollected()
    {
        txt.gameObject.SetActive(false);
        //SoldierSpawner.I.SpawnSoldier(Owner, Owner.GetSoldierCount() * (value - 1));
    }
    //LEVEL UP
    void LevelUpProps()
    {
        txt.gameObject.SetActive(false);
        goUpgrade.SetActive(true);
        onCollected = LevelUpCollected;
    }
    void LevelUpCollected()
    {
        txt.gameObject.SetActive(false);
        goUpgrade.SetActive(false);
        //Owner.LevelUpTroops();
        //if (Owner.IsPlayer())
        //{
        //    //SoundManager.I.PlayOnce(Sounds.Upgrade);
        //}
    }
    //DIVIDE
    //HEALTH
    //MERGE
    //EMPTY
    void EmptyProps()
    {
        smr.enabled = false;
        onCollected = null;
        txt.gameObject.SetActive(false);
        //CloseNeighbourBorders();
    }

    #endregion

    #region CONVERTION


    void SetConvertionVisualPercentage(float per)
    {
        smr.SetBlendShapeWeight(0, 100 - (per * 100));
    }

    Tween stepTween = null, stepScaleTween = null;
    void StepDownAnim()
    {

        if (stepTween != null)
        {
            stepTween.Kill(true);
        }
        if (stepScaleTween != null)
        {
            stepScaleTween.Kill(true);
            stepScaleTween = null;
        }
        float x = 100;

        stepScaleTween = transform.DOScale(.85f, .15f).SetEase(Ease.InOutBounce).OnComplete(() => {
            stepScaleTween = transform.DOScale(1f, .15f).SetEase(Ease.InOutBounce).OnComplete(() => stepScaleTween = null);
        });
        stepTween = DOTween.To(() => x, _x => x = _x, 0, .2f).OnUpdate(() =>
        {
            smr.SetBlendShapeWeight(1, x);
        }).OnComplete(() =>
        {
            smr.SetBlendShapeWeight(1, 0);
            stepTween = null;
        });

    }

    void StepUpAnim()
    {
        if (stepTween != null)
        {
            stepTween.Kill(true);
        }

        float x = 0;

        stepTween = DOTween.To(() => x, _x => x = _x, 100, .2f).OnUpdate(() =>
        {
            smr.SetBlendShapeWeight(1, x);
        }).OnComplete(() =>
        {
            smr.SetBlendShapeWeight(1, 100);
            stepTween = null;
        });
    }

   

    void PauseCountDown()
    {

    }

    void StopCountDown()
    {
        _timerConvertion = 0f;
        SetConvertionVisualPercentage(0f);
       
    }

    void ResumeCountDown()
    {

    }

    void OnConverted()
    {
        float initialY = transform.position.y;
        transform.DOMoveY(initialY + .3f, .1f).OnComplete(() =>
        {
            transform.DOMoveY(initialY, .1f);
        });

        transform.DOScale(1.1f, .1f).OnComplete(() =>
        {
            transform.DOScale(1f, .1f);
        });

        psConvert.Play();
    }

    #endregion

    #region Coloring

    void SetColorOfOwner()
    {
        smr.SetBlendShapeWeight(0, 100);

        //if (Owner != null)
        //{
        //    smr.materials[0].color = Owner.GetHexBaseColor();
        //}
        //else
        //{
        //    smr.materials[0].color = Color.white;
        //}
    }

    #endregion

    public void EnableBorder(int id)
    {
        borders[id].SetActive(true);
    }

    //void CloseNeighbourBorders()
    //{

    //    HexSpawner hs; /*= LevelHandler.I.GetLevel().hexSpawner;*/

    //    if (z % 2 == 0)
    //    {
    //        if (x - 1 >= 0)
    //        {
    //            hs.GetHexAt(x - 1, z).EnableBorder(1);


    //            if (z - 1 >= 0)
    //            {
    //                hs.GetHexAt(x - 1, z - 1).EnableBorder(0);

    //            }
    //            if (z + 1 < hs.zSize)
    //            {
    //                hs.GetHexAt(x - 1, z + 1).EnableBorder(2);
    //            }

    //        }
    //        if (x + 1 < hs.xSize)
    //        {
    //            hs.GetHexAt(x + 1, z).EnableBorder(4);
    //        }
    //        if (z - 1 >= 0)
    //        {
    //            hs.GetHexAt(x, z - 1).EnableBorder(5);
    //        }
    //        if (z + 1 < hs.zSize)
    //        {
    //            hs.GetHexAt(x, z + 1).EnableBorder(3);
    //        }
    //    }
    //    else
    //    {
    //        if (x + 1 < hs.xSize)
    //        {
    //            hs.GetHexAt(x + 1, z).EnableBorder(4);

    //            if (z + 1 < hs.zSize)
    //            {
    //                hs.GetHexAt(x + 1, z + 1).EnableBorder(3);
    //            }

    //            if (z - 1 >= 0)
    //            {
    //                hs.GetHexAt(x + 1, z - 1).EnableBorder(5);
    //            }
    //        }
    //        if (z + 1 < hs.zSize)
    //        {
    //            hs.GetHexAt(x, z + 1).EnableBorder(2);
    //        }
    //        if (x - 1 >= 0)
    //        {
    //            hs.GetHexAt(x - 1, z).EnableBorder(1);
    //        }
    //        if (z - 1 >= 0)
    //        {
    //            hs.GetHexAt(x, z - 1).EnableBorder(0);
    //        }
    //    }
    //}

    public void SetOutsideHex()
    {

        smr.materials[0].color = Color.gray;
        colTrigger.enabled = false;
    }
    private IMemoryPool _pool;

    //public Transform ItemHolder => _itemHolder;

    #region GridState

    public GridState _gridState;

    public GridState GridState
    {
        get => _gridState;
        set
        {
            _gridState = value;

        }
    }

    #endregion

    #region Injection

    private SignalBus _signalBus;
    private PlayerMobController _playerMobController;
    private BoardFXController _boardFXController;

    [Inject]
    private void Construct(SignalBus signalBus
        , PlayerMobController playerMobController
        , BoardFXController boardFXController)
    {
        _signalBus = signalBus;
        _playerMobController = playerMobController;
        _boardFXController = boardFXController;
    }

    #endregion

    public void Init(GridData gridData)
    {
        Initialize();
    }

    
    public override void Initialize()
    {
        
    }
    public void DespawnItem()
    {
        
    }
    public void OnDespawned()
    {
        
    }
    public void OnSpawned(Args args, IMemoryPool pool)
    {
        _pool = pool;
        transform.SetParent(args.parent);
        //_spriteRenderer.sortingOrder = GlobalConsts.SortingOrders.GridViewDefault;
        transform.localScale = args.localScale;
       
        var pos = new Vector3(
            args.offSet.x + args.coloumValue * args.localScale.x,
            args.offSet.y + args.rowValue * args.localScale.y,
            args.offSet.z
        );

        transform.localPosition = pos;

    }
    
    public void ChangeState(GridState gridState) => _gridState = gridState;
    
    public void SetSprite(Sprite sprite)
    {
        //_spriteRenderer.sprite = sprite;
        

    }
    public void SelectGrid(bool animate)
    {
        //_selectionFrame.SetActive(true);

        //if (animate)
        //{
        //    _selectionAnimation.Stop();
        //    _itemSpawnAnimation.Stop();
        //    _selectionAnimation.Play();
        //    _itemSpawnAnimation.Play();
        //}
    }

    public void UnSelectGrid()
    {
        //_selectionFrame.SetActive(false);
    }

    public void PlayVFX(VFXType vfxType)
    {
        _boardFXController.PlayVFX(new VFXArgs(vfxType, transform, 2f));
    }

    public class Factory : PlaceholderFactory<Args, GridView> { }
    public class Pool : MonoPoolableMemoryPool<Args, IMemoryPool, GridView> { }
    public readonly struct Args
    {

        public readonly Transform parent;
        public readonly Vector3 localScale;
        public readonly Vector3 offSet;
        public readonly int rowValue;
        public readonly int coloumValue;
        
               
        public Args(Transform Parent,Vector3 LocalScale,Vector3 OffSet, int RowValue, int ColoumValue) : this()
        {
            parent = Parent;
            localScale = LocalScale;
            offSet = OffSet;
            rowValue = RowValue;
            coloumValue = ColoumValue;
        }
    }
}
