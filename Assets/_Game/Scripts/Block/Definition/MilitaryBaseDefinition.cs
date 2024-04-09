using MyProject.Core.Enums;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(MilitaryBaseDefinition), menuName = AssetMenuName.DEFINITION + nameof(MilitaryBaseDefinition))]
    public class MilitaryBaseDefinition : BaseBlockEntityTypeDefinition
    {
        [SerializeField] protected MilitaryBaseType millitaryBaseType;
        public MilitaryBaseType MillitaryBaseType => millitaryBaseType;
    }
}


