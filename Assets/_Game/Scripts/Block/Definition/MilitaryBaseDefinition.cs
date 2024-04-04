using MyProject.Core.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

[CreateAssetMenu(fileName = nameof(MilitaryBaseDefinition), menuName = AssetMenuName.DEFINITION + nameof(MilitaryBaseDefinition))]
public class MilitaryBaseDefinition : BaseBlockEntityTypeDefinition
{
    [SerializeField] protected MilitaryBaseType millitaryBaseType;
    public MilitaryBaseType MillitaryBaseType => millitaryBaseType;
}

