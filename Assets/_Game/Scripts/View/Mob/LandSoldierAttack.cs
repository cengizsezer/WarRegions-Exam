using MyProject.GamePlay.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandSoldierAttack : AttackBase
{
    public override void Cast()
    {
        parentMob.CurrentAnimator.SetTrigger("Attack");
    }

    public void Attack()
    {
        MobView target = parentMob.GetTarget();

        Damageable d = target.Damageable;

        if (d != null)
        {

            Damageable.DamageMessage message = new Damageable.DamageMessage
            {
                Damager = parentMob,
                Damage = parentMob.Damage,
            };

            d.ApplyDamage(message);
        }
    }
}