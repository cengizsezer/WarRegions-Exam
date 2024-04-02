using MyProject.Core.Enums;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttackView : AttackBaseView
{
    [SerializeField] private ParticleSystem _psImpact;
    public override void Cast()
    {
        Owner._currentAnimator.SetTrigger(AnimStates.Attack.ToString());
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
