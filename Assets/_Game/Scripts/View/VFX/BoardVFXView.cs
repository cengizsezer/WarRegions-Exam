using MyProject.Core.Enums;
using MyProject.Core.Services;
using MyProject.GamePlay.Controllers;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BoardVFXView : BaseView, IVFX, IPoolable<VFXArgs, IMemoryPool>
{
    [SerializeField] private List<VFXTuple> _vfxTuples;

    private IMemoryPool _pool;
    private CoroutineHandle _coroutineHandle;
    private VFXArgs _args;

    #region Injection

    private CoroutineService _coroutineService;

    [Inject]
    private void Construct(CoroutineService coroutineService)
    {
        _coroutineService = coroutineService;
    }

    #endregion

    public void OnSpawned(VFXArgs args, IMemoryPool pool)
    {
        _args = args;
        _pool = pool;
        Initialize();
    }

    public override void Initialize()
    {
        SetTuple(_args.VFXType);
        transform.SetParent(_args.Parent);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        _coroutineHandle = _coroutineService.StartCoroutine(Despawn());
    }

    private void SetTuple(VFXType vfxType)
    {
        foreach (var tuple in _vfxTuples)
        {
            tuple.Transform.gameObject.SetActive(tuple.VFXType == vfxType);
        }
    }

    private IEnumerator<float> Despawn()
    {
        yield return Timing.WaitForSeconds(_args.Time);
        _pool.Despawn(this);
    }

    public void OnDespawned()
    {
        if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
    }

    private void OnApplicationQuit()
    {
        if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
    }

    public class Factory : PlaceholderFactory<VFXArgs, BoardVFXView>
    {
    }

    public class Pool : MonoPoolableMemoryPool<VFXArgs, IMemoryPool, BoardVFXView>
    {
    }
}

[Serializable]
public class VFXTuple
{
    public VFXType VFXType;
    public Transform Transform;
}
