using MyProject.GamePlay.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    public MobView parentMob;
    public abstract void Cast();
}
