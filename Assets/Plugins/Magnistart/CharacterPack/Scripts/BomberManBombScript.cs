using UnityEngine;

namespace Magnistart_CharacterPack
{
    public class BomberManBombScript : MonoBehaviour
    {
        public Transform explosionFx = null;
        //This will be called by Animator.
        public void Destroy()
        {
            //Generate an explosion at bomb sprite position
            Instantiate(explosionFx, transform.GetChild(0).position, explosionFx.localRotation);
            Destroy(gameObject);
        }
    }
}