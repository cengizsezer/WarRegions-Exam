using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MyProject.Core.Enums;
using DG.Tweening;
using TMPro;

public class MillitaryBaseView : BaseView, IPoolable<MillitaryBaseView.Args, IMemoryPool>
{
    [SerializeField] MeshRenderer[] arrMeshRenderer;
    [HideInInspector] public GridView Owner;
    private MeshRenderer currentRenderer;
   
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

    private int IncreaseValue = 5;
    private int power;
    public int Power
    {
        get => power;

        set
        {
            power = value;

            if(power<0)
            {
                OnDeath();
                return;
            }
            txt.text = power.ToString();


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
                arrMeshRenderer[i].material.color = Owner.GetSmr().material.color;
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
            Power += IncreaseValue;
            StartTentCooldown();
           

        }).SetEase(Ease.Linear);


    }

    public void Attack()
    {
       // type göre asker cıkar
    }

    public void OnDeath()
    {
        //renk Değişecek bunun ve tüm bölgenin
    }

    public void OnrHit(int HitValue)
    {
        power -= HitValue;
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

        public Args(Transform Parent, Vector3 LocalScale,Vector3 position,GridView owner) : this()
        {
            parent = Parent;
            localScale = LocalScale;
            Position = position;
            Owner = owner;
           
        }
    }

   
}
