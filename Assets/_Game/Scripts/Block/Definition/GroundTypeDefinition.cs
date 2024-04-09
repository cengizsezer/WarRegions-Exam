using MyProject.Core.Data;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(GroundTypeDefinition), menuName = AssetMenuName.DEFINITION + nameof(GroundTypeDefinition))]
    public class GroundTypeDefinition : BaseBlockEntityTypeDefinition
    {
        [SerializeField] protected ResourceTypeData resourceTypeData;
        public ResourceTypeData ResourceTypeData => resourceTypeData;


    }

}


