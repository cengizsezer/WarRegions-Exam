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


    [SerializeField] MeshRenderer[] arrMeshRenderer;
    public GridView Owner;
    private MeshRenderer currentRenderer;
    public Region Region;
    private UserType userType;
    public UserType UserType
    {
        get => userType;

        set
        {
            userType = value;

        }
    }
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

    private int soldierIncreaseValue = 5;
    private int soldierCount;
    public int SoldierCount
    {
        get => soldierCount;

        set
        {
            soldierCount = value;
            txt.text = soldierCount.ToString();
        }
    }
   
    private IMemoryPool _pool;

    public UserType GetUserType() => userType;
    void SetMeshRenderer(int ID)
    {
        int _id = ID;

        for (int i = 0; i < arrMeshRenderer.Length; i++)
        {
            if (i == _id)
            {
                arrMeshRenderer[i].gameObject.SetActive(i == _id);
                currentRenderer = arrMeshRenderer[i];
                currentRenderer.material.color = Owner.GetSmr().material.color;
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

        tentSpawnTween = DOTween.To(() => fillAmount, _x => fillAmount = _x, 0, 5f).OnUpdate(() =>
        {
            
                tentSprite.material.SetFloat("_Arc2", fillAmount);
        }).OnComplete(() =>
        {
            
            tentSpawnTween = null;
            SoldierCount += soldierIncreaseValue;
            StartTentCooldown();
           

        }).SetEase(Ease.Linear);


    }
    

    public void Attack(MillitaryBaseView other)
    {
        var path = new List<GridView>();
        path = _pathFinderController.FindGridPath(this.Owner,other.Owner);
        MobView mob=_playerMobController.CreateMobView();
        mob.SetPropsView(new SoldierWarData
        {
            SpawnPosition = transform.position,
            color = Owner.GetSmr().material.color,
            MobBlockType = Owner.mType,
            SoldierCount = SoldierCount,
            TargetMilitaryBase = other,
            OwnerMilitaryBase=this,
            Path = path

        });
        mob.Initialize();

        SendingTroops();
    }

    public void SendingTroops()
    {
        SoldierCount = 0;
    }
    public void TakeOver(int count,MobView mob)
    {
        if(Owner.mType == BlockType.Gray)
        {
            soldierCount = mob.GetSoldierCount();

            foreach (var grid in Region.RegionPairs)
            {
                grid.SetColorColor(mob.OwnerMilitaryBaseView.Owner.GetSmr().material.color);
            }

            currentRenderer.material.color = mob.OwnerMilitaryBaseView.Owner.GetSmr().material.color;
            Debug.Log("girdi ama nerede");

        }
        else
        {
            Debug.Log("girdi ama nerede", Owner.gameObject);

            SoldierCount -= count;

            if (soldierCount < 0)
            {
                soldierCount = mob.GetSoldierCount();

                foreach (var grid in Region.RegionPairs)
                {
                    grid.SetColorColor(mob.OwnerMilitaryBaseView.Owner.GetSmr().material.color);
                }

                currentRenderer.material.color = mob.OwnerMilitaryBaseView.Owner.GetSmr().material.color;

            }
        }

       

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
    void TentClosed()
    {
        txt.gameObject.SetActive(false);
        currentRenderer.gameObject.SetActive(true);
        tentSprite.gameObject.SetActive(false);
       
    }
    void OnTentWorking()
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
    public void SelectView()
    {

    }
    public void UnSelectView()
    {

    }
    public override void Initialize()
    {
        if (Owner.GetTileType() == BlockType.Gray) return;

        OnTentWorking();
    }
    

    public void OnSpawned(Args args, IMemoryPool pool)
    {
        _pool = pool;
        transform.SetParent(args.parent);
        transform.localScale=args.localScale;
        transform.position = args.Position;
        Owner = args.Owner;
       
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
