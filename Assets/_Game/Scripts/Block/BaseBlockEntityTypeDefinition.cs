using UnityEngine;

namespace MyProject.Core.Settings
{
    public class BaseBlockEntityTypeDefinition : ScriptableObject, IBlockEntityTypeDefinition
    {

        [SerializeField] protected string defaultEntitySpriteName;
        [SerializeField] protected Sprite helperSprite;

        public string DefaultEntitySpriteName => defaultEntitySpriteName;
        public Sprite HelperSprite => helperSprite;
    }
}



