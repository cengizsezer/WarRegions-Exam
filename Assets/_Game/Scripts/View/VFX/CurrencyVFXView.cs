using MyProject.Core.Enums;
using MyProject.Core.Services;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public partial class CurrencyVFXView : BaseView, IPoolable<CurrencyVFXView.Args, IMemoryPool>
{
    public class Factory : PlaceholderFactory<Args, CurrencyVFXView> { }
    public class Pool : MonoPoolableMemoryPool<Args, IMemoryPool, CurrencyVFXView> { } 

    public readonly struct Args
    {
        public readonly Vector2 StartPosition;
        public readonly CurrencyType CurrencyType;
        public readonly int Amount;
        public readonly float Duration;


        public Args(Vector2 startPosition, CurrencyType currencyType, int amount, float duration) : this()
        {
            StartPosition = startPosition;
            CurrencyType = currencyType;
            Amount = amount;
            Duration = duration;
        }
    }
}

public partial class CurrencyVFXView : BaseView, IPoolable<CurrencyVFXView.Args, IMemoryPool>
{
    private IMemoryPool _pool;

    [SerializeField] private ParticleSystem coinParticle;
    [SerializeField] private ParticleSystem manaParticle;
   

    private CoroutineHandle _coroutineHandle;


    #region Injection

    private CoroutineService _coroutineService;

    [Inject]
    private void Construct(CoroutineService coroutineService)
    {
        _coroutineService = coroutineService;
    }

    #endregion

    private IEnumerator<float> Despawn()
    {
        yield return Timing.WaitForSeconds(3f);
        _pool.Despawn(this);
    }

    public void OnSpawned(Args args, IMemoryPool pool)
    {
        _pool = pool;

        transform.localPosition = new Vector3(args.StartPosition.x, args.StartPosition.y, 100f);

        var burst = new ParticleSystem.Burst
        {
            count = args.Amount
        };

        switch (args.CurrencyType)
        {
            case CurrencyType.Coin:
                coinParticle.emission.SetBurst(0, burst);
                coinParticle.gameObject.SetActive(true);
                break;
            case CurrencyType.Mana:
                manaParticle.emission.SetBurst(0, burst);
                manaParticle.gameObject.SetActive(true);
                break;
        }

        _coroutineHandle = _coroutineService.StartCoroutine(Despawn());
    }


    public override void Initialize()
    {
    }

    public void OnDespawned()
    {
        coinParticle.gameObject.SetActive(false);
        manaParticle.gameObject.SetActive(false);

    }

    private void OnApplicationQuit()
    {
        if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
    }


    
}
