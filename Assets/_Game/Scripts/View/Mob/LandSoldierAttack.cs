using MyProject.GamePlay.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandSoldierAttack : AttackBase
{
    public override void Cast()
    {
        //parentMob.crntAnim.SetTrigger("Attack");
    }

    public void Attack()
    {
        MobView target = parentMob.GetTarget();
    }
}
