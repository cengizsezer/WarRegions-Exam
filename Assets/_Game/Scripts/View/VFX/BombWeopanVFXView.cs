using MyProject.Core.Const;
using MyProject.Core.Services;
using DG.Tweening;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BombWeopanVFXView : BaseWeopanVFXView, IPoolable<IMemoryPool>
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    
    private IMemoryPool _pool;
    private CoroutineHandle _coroutineHandle;
    
    #region Injection

    private CoroutineService _coroutineService;
    private TimerService _timerService;
    [Inject]
    private void Construct
    (
          CoroutineService coroutineService
        , TimerService timerService
    )
    {
        _coroutineService = coroutineService;
        _timerService = timerService;
    }

    #endregion
    public override void SetView(Sprite sprite) => _spriteRenderer.sprite = sprite;
   
    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
        Initialize();
    }

    public override void Initialize()
    {
        _coroutineHandle = _coroutineService.StartCoroutine(Despawn());
    }
    private IEnumerator<float> Despawn()
    {
        _spriteRenderer.sortingOrder = GlobalConsts.SortingOrders.CharacterDefault;
        yield return Timing.WaitForSeconds(2f);
        //_pool.Despawn(this);
    }
    public void OnDespawned()
    {
       
        if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
    }
    private void OnApplicationQuit()
    {
        if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
    }

    public override void VoidDespawn()
    {
        DOTween.Kill(transform);
        _pool.Despawn(this);
    }

    public class Factory : PlaceholderFactory<BombWeopanVFXView>
    {
    }
    public class Pool : MonoPoolableMemoryPool<IMemoryPool, BombWeopanVFXView>
    {
    }
}
