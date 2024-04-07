using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MyProject.Core.Enums;
using DG.Tweening;
using TMPro;
using MyProject.GamePlay.Controllers;
using MyProject.GamePlay.Characters;
using MyProject.Core.Data;
using MyProject.Core.Const;

[System.Serializable]
public class Region
{
    public List<GridView> RegionPairs;

    public Region()
    {
        RegionPairs = new();
    }

}
public class MilitaryBaseView : BaseView, IPoolable<MilitaryBaseView.Args, IMemoryPool>
{
    private IMemoryPool _pool;
    [SerializeField] private ResourceTypeData _resourceTypeData;
    public ResourceTypeData ResourceTypeData
    {
        get => _resourceTypeData;
        set
        {
            _resourceTypeData = value;
            SetProps();
        }
    }

    #region Injection

    private SignalBus _signalBus;
    private PlayerMobController _MobController;
    private PathFinderController _pathFinderController;
    private BoardCoordinateSystem _boardCoordinateSystem;
    private BoardGamePlayController _boardGamePlayController;
    

    [Inject]
    private void Construct
    (     SignalBus signalBus,
          PlayerMobController playerMobController
        , PathFinderController pathFinderController
        , BoardCoordinateSystem boardCoordinateSystem
        ,BoardGamePlayController boardGamePlayController)
    {
        _signalBus = signalBus;
        _MobController = playerMobController;
        _pathFinderController = pathFinderController;
        _boardCoordinateSystem = boardCoordinateSystem;
        _boardGamePlayController = boardGamePlayController;
    }

    #endregion

    #region ENUMS
    [SerializeField]private ColorType _colorType;
    public int ConfigureType;
    public ColorType GetColorType() => _colorType;

    [SerializeField]private UserType _userType;
    public UserType UserType
    {
        get => _userType;

        set
        {
            _userType = value;
            _boardCoordinateSystem.AddGamePlayList(this);

        }
    }
    public UserType GetUserType() => _userType;

    private MilitaryBaseType militaryBaseType;
    public MilitaryBaseType MilitaryBaseType
    {
        get => militaryBaseType;

        set
        {
            militaryBaseType = value;
            SetMeshRenderer((int)militaryBaseType);


        }
    }

    #endregion

    [SerializeField] MeshRenderer[] arrMeshRenderer;
    private MeshRenderer currentRenderer;

    public GridView Owner;
    public Region Region;

    private int _soldierIncreaseValue;
    private float _soldierWaitingTime;
    private int _soldierMaxCount;
    private int _soldierCount;
    public int SoldierCount
    {
        get => _soldierCount;

        set
        {
            _soldierCount = value;
            txt.text = _soldierCount.ToString();
            if (_soldierCount < _soldierMaxCount && GetUserType()!=UserType.Nötr)
            {
                StartTentCooldown();
            }
           

        }
    }
   
    public void SetSettingsCountAndValues(int defaultCount,float waitingTime,int increaseValue,int maxCount)
    {
        SoldierCount = defaultCount;
        _soldierWaitingTime = waitingTime;
        _soldierIncreaseValue = increaseValue;
        _soldierMaxCount = maxCount;
       
    }
    void SetMeshRenderer(int ID)
    {
        int _id = ID;

        for (int i = 0; i < arrMeshRenderer.Length; i++)
        {
            arrMeshRenderer[i].gameObject.SetActive(false);
            if (i == _id)
            {
                arrMeshRenderer[i].gameObject.SetActive(i == _id);
                currentRenderer = arrMeshRenderer[i];
               
            }
        }
    }

    private void SetProps()
    {
        foreach (var material in currentRenderer.materials)
        {
            material.color = ToColorFromHex(ResourceTypeData.HexColor);
        }

        txt.color = ToColorFromHex(ResourceTypeData.HexColor);
        
        _colorType = ResourceTypeData.ColorType;
        ConfigureType = ResourceTypeData.ConfigureType;
    }


    #region TentControl

    Tween tentSpawnTween = null;
    [SerializeField] SpriteRenderer tentSprite;
    [SerializeField] TextMeshPro txt;
    private void StartTentCooldown()
    {
       

        if (tentSpawnTween != null)
        {
            tentSpawnTween.Kill(false);
            tentSpawnTween = null;
        }
        float fillAmount = 360f;
        tentSprite.material.SetFloat("_Arc2", fillAmount);

        tentSpawnTween = DOTween.To(() => fillAmount, _x => fillAmount = _x, 0, _soldierWaitingTime).OnUpdate(() =>
        {
            
                tentSprite.material.SetFloat("_Arc2", fillAmount);
        }).OnComplete(() =>
        {
            
            tentSpawnTween = null;
            SoldierCount += _soldierIncreaseValue;

            if(_soldierCount<_soldierMaxCount)
            {
                StartTentCooldown();

            }
            else
            {
                StopTentCooldown();
            }
           
           

        }).SetEase(Ease.Linear);


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
    private void OnTentWorking()
    {
        Vector3 localScale = currentRenderer.transform.localScale;
        float sclae = currentRenderer.transform.localScale.y;
        tentSprite.gameObject.SetActive(true);
        currentRenderer.transform.DOScale(sclae * 1.12f, .18f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            currentRenderer.transform.DOScale(localScale, .2f).SetEase(Ease.InBounce);
        });
        currentRenderer.transform.localScale = localScale;
        StartTentCooldown();
    }

    private void CloseTent()
    {
        txt.gameObject.SetActive(false);
        StopTentCooldown();
        tentSpawnTween.Kill(false);
        tentSpawnTween = null;
        tentSprite.gameObject.SetActive(false);
    }

    #endregion

    public void SendingTroops(MilitaryBaseView other)
    {
        if(MilitaryBaseType==MilitaryBaseType.Sea)
        {
            _boardCoordinateSystem.CalculateNeigbor(true);
        }
        else
        {
            _boardCoordinateSystem.CalculateNeigbor(false);
        }
      
        var path = new List<GridView>();

        path = _pathFinderController.FindGridPath(this.Owner, other.Owner);
        MobView mob = _MobController.CreateMobView();


        mob.SetPropsView(new SoldierWarData
        {
            MilitaryBaseType = MilitaryBaseType,
            SpawnPosition = transform.position,
            ResourceTypeData = ResourceTypeData,
            SoldierCount = SoldierCount,
            TargetMilitaryBase = other,
            OwnerMilitaryBase = this,
            Path = path

        });

        _MobController.LsMobViews.Add(mob);
        mob.Initialize();

        SoldierCount = 0;
        
    }
    public void TakeOver(MobView mob)
    {
        switch (GetUserType())
        {
            case UserType.Player:

                if(mob.OwnerMilitaryBaseView.GetUserType()==UserType.Player)
                {
                    SoldierCount += mob.GetSoldierCount();
                    return;
                }
               
                if (SoldierCount >= mob.GetSoldierCount())
                {
                    SoldierCount -= mob.GetSoldierCount();
                    return;
                }
                else
                {
                    var value = mob.GetSoldierCount() - SoldierCount;
                    SoldierCount = value;
                    Region.RegionPairs.ForEach(n => n.ResourceTypeData = mob.OwnerMilitaryBaseView.ResourceTypeData);
                    ResourceTypeData = mob.OwnerMilitaryBaseView.ResourceTypeData;
                    UserType = mob.OwnerMilitaryBaseView.UserType;
                    _boardCoordinateSystem.lsEnemyMilitaryBaseView.Remove(this);

                }
                break;
            case UserType.Enemy:

                if (mob.OwnerMilitaryBaseView.GetUserType() == UserType.Enemy)
                {
                    SoldierCount += mob.GetSoldierCount();
                    return;
                }

                if (SoldierCount >= mob.GetSoldierCount())
                {
                    SoldierCount -= mob.GetSoldierCount();
                    return;
                }
                else
                {
                    var value = mob.GetSoldierCount() - SoldierCount;
                    SoldierCount = value;
                    Region.RegionPairs.ForEach(n => n.ResourceTypeData = mob.OwnerMilitaryBaseView.ResourceTypeData);
                    ResourceTypeData = mob.OwnerMilitaryBaseView.ResourceTypeData;
                    UserType = mob.OwnerMilitaryBaseView.UserType;
                    _boardCoordinateSystem.lsEnemyMilitaryBaseView.Remove(this);

                }
                break;
            case UserType.Nötr:
                if (SoldierCount >= mob.GetSoldierCount())
                {
                    SoldierCount -= mob.GetSoldierCount();
                    Debug.Log(SoldierCount);
                    return;
                }
                else
                {
                    var value = mob.GetSoldierCount() - SoldierCount;

                    SoldierCount = value;
                    OnTentWorking();
                    Region.RegionPairs.ForEach(n => n.ResourceTypeData = mob.OwnerMilitaryBaseView.ResourceTypeData);
                    ResourceTypeData = mob.OwnerMilitaryBaseView.ResourceTypeData;
                    UserType = mob.OwnerMilitaryBaseView.UserType;
                   
                }
                break;
            default:
                break;
              
        }

        _boardGamePlayController.CheckLevelControl();
    }
    public void SelectView()
    {

    }
    public void UnSelectView()
    {

    }
    public override void Initialize()
    {
       
        if (Owner.TypeDefinition.DefaultEntitySpriteName == GlobalConsts.RegionName.GRAY) return;

        OnTentWorking();
    }
    public void OnSpawned(Args args, IMemoryPool pool)
    {
        _signalBus.Subscribe<LevelFailSignal>(CloseTent);
        _signalBus.Subscribe<LevelSuccessSignal>(CloseTent);
        _pool = pool;
        transform.SetParent(args.parent);
        transform.localScale=args.localScale;
        transform.position = args.Position;
        MilitaryBaseType = args.MilitaryBaseType;
        Owner = args.Owner;
        Region = args.Region;
        ResourceTypeData = Owner.ResourceTypeData;


    }
    public void DespawnItem()
    {
        if (!this.gameObject.activeInHierarchy)
            return;
       
        _signalBus.TryUnsubscribe<LevelFailSignal>(CloseTent);
        _signalBus.TryUnsubscribe<LevelSuccessSignal>(CloseTent);
        _pool.Despawn(this);
    }
    public void OnDespawned()
    {
       
    }

    public class Factory : PlaceholderFactory<Args, MilitaryBaseView> { }
    public class Pool : MonoPoolableMemoryPool<Args, IMemoryPool, MilitaryBaseView> { }
    public readonly struct Args
    {
        public readonly Transform parent;
        public readonly Vector3 localScale;
        public readonly Vector3 Position;
        public readonly GridView Owner;
        public readonly Region Region;
        public readonly MilitaryBaseType MilitaryBaseType;

        public Args(Transform Parent, Vector3 LocalScale,Vector3 position, MilitaryBaseType militaryBaseType, GridView owner,Region region) : this()
        {
            parent = Parent;
            localScale = LocalScale;
            Position = position;
            Owner = owner;
            Region = region;
            MilitaryBaseType = militaryBaseType;


        }
    }

   
}
