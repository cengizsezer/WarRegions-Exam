using MyProject.Core.Enums;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

[CreateAssetMenu(fileName = nameof(GroundTypeDefinition), menuName = AssetMenuName.DEFINITION + nameof(GroundTypeDefinition))]
public class GroundTypeDefinition : BaseBlockEntityTypeDefinition
{
    [SerializeField] protected ResourceTypeData resourceTypeData;
    public ResourceTypeData ResourceTypeData => resourceTypeData;
    
    
}


