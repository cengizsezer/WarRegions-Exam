using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

[CreateAssetMenu(fileName = nameof(GroundTypeDefinition), menuName = AssetMenuName.DEFINITION + nameof(GroundTypeDefinition))]
public class GroundTypeDefinition : BaseBlockEntityTypeDefinition
{
    [SerializeField] protected BlockType blockType;
    [SerializeField] protected string rgbColor;
    public BlockType BlockType => blockType;
    public string RGBColor => rgbColor;
    
}

public enum BlockType
{
    Yellow,
    Red,
    Blue,
    Gray
}
