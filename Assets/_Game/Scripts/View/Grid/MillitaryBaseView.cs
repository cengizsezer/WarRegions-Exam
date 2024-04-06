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
public class MillitaryBaseView : BaseView, IPoolable<MillitaryBaseView.Args, IMemoryPool>
{
    private IMemoryPool _pool;
    #region Injection

    private SignalBus _signalBus;
    private PlayerMobController _playerMobController;
    private PathFinderController _pathFinderController;
    private BoardCoordinateSystem _boardCoordinateSystem;
    

    [Inject]
    private void Construct
    (     SignalBus signalBus,
          PlayerMobController playerMobController
        , PathFinderController pathFinderController
        , BoardCoordinateSystem boardCoordinateSystem)
    {
        _signalBus = signalBus;
        _playerMobController = playerMobController;
        _pathFinderController = pathFinderController;
        _boardCoordinateSystem = boardCoordinateSystem;
    }

    #endregion

    #region ENUMS
    private BlockType blockType;
    public BlockType GetBlockType() => blockType;

    private UserType userType;
    public UserType UserType
    {
        get => userType;

        set
        {
            userType = value;

        }
    }
    public UserType GetUserType() => userType;

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
            if (_soldierCount < _soldierMaxCount)
            {
                StartTentCooldown();
            }
           

        }
    }
   
   
    private Color _currentColor;
    public Color GetColor() => _currentColor;
   
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
            if (i == _id)
            {
                arrMeshRenderer[i].gameObject.SetActive(i == _id);
                currentRenderer = arrMeshRenderer[i];
                _currentColor = Owner.GetSmr().material.color;
                currentRenderer.material.color = _currentColor;
            }
        }
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
        txt.gameObject.SetActive(true);
        //currentRenderer.material.DOColor((UserType==UserType.Player ? Color.blue : Color.red), .3f);
        currentRenderer.transform.DOScale(sclae * 1.12f, .18f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            currentRenderer.transform.DOScale(localScale, .2f).SetEase(Ease.InBounce);
        });
        currentRenderer.transform.localScale = localScale;
        StartTentCooldown();
    }

    #endregion

    public void SendingTroops(MillitaryBaseView other)
    {
        var path = new List<GridView>();

        path = _pathFinderController.FindGridPath(this.Owner, other.Owner);
        MobView mob = _playerMobController.CreateMobView();
        mob.SetPropsView(new SoldierWarData
        {
            MilitaryBaseType = MilitaryBaseType,
            SpawnPosition = transform.position,
            color = Owner.GetSmr().material.color,
            MobBlockType = Owner.mType,
            SoldierCount = SoldierCount,
            TargetMilitaryBase = other,
            OwnerMilitaryBase = this,
            Path = path

        });
        mob.Initialize();

        SoldierCount = 0;
        
    }
    public void TakeOver(MobView mob)
    {
        if (Owner.TypeDefinition.DefaultEntitySpriteName == GlobalConsts.RegionName.GRAY)
        {

            if(SoldierCount >= mob.GetSoldierCount())
            {
                SoldierCount -= mob.GetSoldierCount();
                return;
            }
            else
            {
                var value=mob.GetSoldierCount() - SoldierCount;
                Debug.Log("VALUE"+"-----------"+value);
                SoldierCount = value;
                OnTentWorking();

                foreach (var grid in Region.RegionPairs)
                {
                    grid.SetColorColor(mob.OwnerMilitaryBaseView.GetColor());
                }

                if (mob.OwnerMilitaryBaseView.userType == UserType.Player)
                    userType = UserType.Player;

                _currentColor = mob.OwnerMilitaryBaseView.GetColor();
                currentRenderer.material.color = _currentColor;
                blockType = mob.OwnerMilitaryBaseView.Owner.mType;
                
            }

           
        }
        else
        {
            if (SoldierCount >= mob.GetSoldierCount())
            {
                SoldierCount -= mob.GetSoldierCount();
                return;
            }
            else
            {
                var value = mob.GetSoldierCount() - SoldierCount;
                SoldierCount = value;
                foreach (var grid in Region.RegionPairs)
                {
                    grid.SetColorColor(mob.OwnerMilitaryBaseView.GetColor());
                }

                if (mob.OwnerMilitaryBaseView.userType == UserType.Player)
                    userType = UserType.Player;
                _currentColor = mob.OwnerMilitaryBaseView.GetColor();
                currentRenderer.material.color = _currentColor;
                blockType = mob.OwnerMilitaryBaseView.Owner.mType;
            }

            

            //if (_soldierCount < 0)
            //{
            //    _soldierCount = mob.GetSoldierCount();

            //    foreach (var grid in Region.RegionPairs)
            //    {
            //        grid.SetColorColor(mob.OwnerMilitaryBaseView.GetColor());
            //    }

            //    if (mob.OwnerMilitaryBaseView.userType == UserType.Player)
            //        userType = UserType.Player;

               
            //}
        }



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
        _pool = pool;
        transform.SetParent(args.parent);
        transform.localScale=args.localScale;
        transform.position = args.Position;
        Owner = args.Owner;
        Region = args.Region;
        blockType = Owner.mType;


    }
    public void OnDespawned()
    {
        _pool.Despawn(this);
    }


    public class Factory : PlaceholderFactory<Args, MillitaryBaseView> { }
    public class Pool : MonoPoolableMemoryPool<Args, IMemoryPool, MillitaryBaseView> { }
    public readonly struct Args
    {
        public readonly Transform parent;
        public readonly Vector3 localScale;
        public readonly Vector3 Position;
        public readonly GridView Owner;
        public readonly Region Region;

        public Args(Transform Parent, Vector3 LocalScale,Vector3 position,GridView owner,Region region) : this()
        {
            parent = Parent;
            localScale = LocalScale;
            Position = position;
            Owner = owner;
            Region = region;
           
        }
    }

   
}
