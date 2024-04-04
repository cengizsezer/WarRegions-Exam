using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MyProject.Core.Enums;

public class MillitaryBaseView : BaseView, IPoolable<MillitaryBaseView.Args, IMemoryPool>
{
    [SerializeField] MeshRenderer[] arrMeshRenderer;
    private MilitaryBaseType militaryBaseType;
    public MilitaryBaseType MilitaryBaseType
    {
        get => militaryBaseType;

        set
        {
            if (militaryBaseType != value)
            {
                militaryBaseType = value;
                SetMeshRenderer((int)militaryBaseType);

            }
        }
    }
   
    private IMemoryPool _pool;
    void SetMeshRenderer(int ID)
    {
        int _id = ID;

        for (int i = 0; i < arrMeshRenderer.Length; i++)
        {
            if (i == _id)
            {
                arrMeshRenderer[i].gameObject.SetActive(i == _id);
            }
        }
    }

    public override void Initialize()
    {

    }
    

    public void OnSpawned(Args args, IMemoryPool pool)
    {
        _pool = pool;
        transform.SetParent(args.parent);
        transform.localScale=args.localScale;
        transform.position = args.Position;
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

        public Args(Transform Parent, Vector3 LocalScale,Vector3 position) : this()
        {
            parent = Parent;
            localScale = LocalScale;
            Position = position;
           
        }
    }

   
}
