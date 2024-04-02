using MyProject.GamePlay.Characters;
using MyProject.GamePlay.Controllers;
using DG.Tweening;
using UnityEngine;
using Zenject;

public enum AttackType
{
    Bow=0,
    Bomb=1,
    Axe=2
}
public abstract class AttackBaseView : BaseView
{
    public AttackType AttackType;
    public Transform WeopanPlace;
    protected BaseWeopanVFXView VFX;
  
    protected EnemyMobView _currentTarget;
    [SerializeField] protected PlayerMobView Owner;

    private WeopanVFXController _weopanVFXController;
    private BoardGamePlayController _boardGamePlayController;
    
    [Inject]
    protected virtual void Construct
    (
       WeopanVFXController weopanVFXController
        ,BoardGamePlayController boardGamePlayController
       
    )
    {
        _weopanVFXController = weopanVFXController;
        _boardGamePlayController = boardGamePlayController;
    }

    public override void Initialize()
    {
       
    }

    public virtual void Draw()
    {
        VFX = _weopanVFXController.GetWeopanElement(AttackType);
        VFX.transform.SetParent(this.WeopanPlace);
        VFX.transform.localPosition = Vector3.zero;
        VFX.transform.localRotation = Quaternion.identity;
        VFX.transform.localScale = Vector3.one*3f;
        Owner.SetWeopan(VFX);
    }

    public abstract void Cast();

    public void Animation(EnemyMobView enemyMob, BaseWeopanVFXView vfx)
    {
        vfx.transform.rotation = Quaternion.identity;
        var ID = vfx.transform;
        float duration = Owner._attackSpeed;

        Tweener t = vfx.transform.DOMove(enemyMob.transform.position, duration).SetEase(Ease.Linear).SetId(ID);

        t.OnUpdate(() =>
        {
            float remainingTime = t.Duration() - t.Elapsed();

            vfx.transform.SlowLookAt(enemyMob.transform.position, remainingTime);
            if (remainingTime <= 0.05f)
            {
                vfx.VoidDespawn();

                if (enemyMob.IsAlive)
                {
                    enemyMob.ReceiveDamage(Owner.Damage);
                    DOTween.Kill(ID);
                }
            }
            else
            {
                if (enemyMob.IsAlive)
                {
                    t.ChangeEndValue(enemyMob.transform.position, remainingTime, true).SetEase(Ease.Linear);
                }
                else
                {
                    DOTween.Kill(ID);
                    vfx.VoidDespawn();
                }
            }
        });
    }
}
