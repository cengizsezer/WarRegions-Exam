using MyProject.Core.Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyBossView : BaseView, IInitializable
{
    [SerializeField] SpriteRenderer _spriteRenderer;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public override void Initialize()
    {
        _signalBus.Subscribe<LevelFailSignal>(Destroy);
        _signalBus.Subscribe<LevelSuccessSignal>(Destroy);
       transform.position = new Vector3(-3f, -3f, -3f);
        _spriteRenderer.sortingOrder = GlobalConsts.SortingOrders.CharacterDefault;
    }

    private void Destroy()
    {
        if (!gameObject.activeSelf) return;
        Destroy(gameObject);
        Dispose();

    }


    public override void Dispose()
    {
        _signalBus.TryUnsubscribe<LevelFailSignal>(Destroy);
        _signalBus.TryUnsubscribe<LevelSuccessSignal>(Destroy);
        base.Dispose();
    }

    public class Factory : PlaceholderFactory<Object, EnemyBossView> { }
}
