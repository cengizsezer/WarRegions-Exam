using MyProject.Core.Enums;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BombAttackView : AttackBaseView
{
    [SerializeField] private ParticleSystem _psImpact;
    [SerializeField] private float radius;
    [SerializeField] private float OffSet;

    public override void Cast()
    {
        Owner._currentAnimator.SetTrigger("Attack");
    }
    public void Attack()
    {
        _currentTarget = Owner.GetTarget();

        if (Owner == null || _currentTarget == null || !_currentTarget.IsAlive)
        {
            if (VFX != null)
            {
                VFX.DestroyView();
            }

            if (Owner != null && Owner._currentAnimator != null)
            {
                Owner._currentAnimator.SetTrigger(AnimStates.Idle.ToString());
            }

            return;
        }

        VFX.transform.SetParent(null);
        VFX.transform.rotation = Quaternion.identity;
        Owner.SetWeopan(null);
        Animation(_currentTarget, VFX);
    }

}
