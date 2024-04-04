using UnityEngine;
using UnityEngine.U2D;

public class BaseBlockEntityTypeDefinition : ScriptableObject, IBlockEntityTypeDefinition
{
   
    [SerializeField] protected string defaultEntitySpriteName;
    [SerializeField] protected Sprite helperSprite;

    public string DefaultEntitySpriteName => defaultEntitySpriteName;
    public Sprite HelperSprite => helperSprite;
}


