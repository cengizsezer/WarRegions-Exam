using MyProject.GamePlay.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaSoldierAttack : AttackBase
{
    public override void Cast()
    {
        //parentMob.crntAnim.SetTrigger("SendingTroops");
    }

    public void Attack()
    {
        MobView target = parentMob.GetTarget();
    }
}
