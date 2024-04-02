using MyProject.Core.Const;
using MyProject.Core.Controllers;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerBossView : BaseView
{
    public bool IsAlive = true;
   
    [SerializeField] SpriteRenderer _spriteRenderer;
    private float _hp;
    public float HP
    {
        get => _hp;

        set
        {
            _hp = value;


            if (_hp <= 0)
            {
                _hp = 0f;
                if (IsAlive)
                {
                    OnDeath();
                }
            }
        }
    }

    #region Injection

    private CharacterItemSettings _characterSettings;
    private UserController _userController;
    private TaskService _taskService;

    [Inject]
    private void Construct(
        CharacterItemSettings characterItemSettings
        ,UserController userController
        ,TaskService taskService)
    {
        _characterSettings = characterItemSettings;
        _userController = userController;
        _taskService = taskService;
    }

    #endregion

    public override void Initialize()
    {
        _signalBus.Subscribe<LevelFailSignal>(Destroy);
        _signalBus.Subscribe<LevelSuccessSignal>(Destroy);
        HP = _characterSettings.PlayerGamingSettings.PlayerBossHP;
        IsAlive = true;
        transform.position = new Vector3(3f, -3f, -3f);
        _spriteRenderer.sortingOrder = GlobalConsts.SortingOrders.CharacterDefault;

    }
    private void Destroy()
    {
        if (!gameObject.activeSelf) return;
        Destroy(gameObject);
        Dispose();

    }
    public void ReceiveDamage(float f)
    {

        HP -= f;
        HP = Mathf.Clamp(HP, 0, _characterSettings.PlayerGamingSettings.PlayerBossHP);

    }
    public virtual void OnDeath()
    {
        if (!IsAlive) return;
        _taskService.AddTask(new LockUIGameTask());
        IsAlive = false;
        _userController.LevelFail();
        DestroyView();
        

    }

    public override void Dispose()
    {
        _signalBus.TryUnsubscribe<LevelFailSignal>(Destroy);
        _signalBus.TryUnsubscribe<LevelSuccessSignal>(Destroy);
        base.Dispose();
    }
    public class Factory : PlaceholderFactory<Object, PlayerBossView> { }
}
