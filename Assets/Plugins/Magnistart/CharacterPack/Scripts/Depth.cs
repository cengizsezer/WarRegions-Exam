using UnityEngine;

namespace Magnistart_CharacterPack
{
    [ExecuteAlways]
    public class Depth : MonoBehaviour
    {
        private void LateUpdate()
        {
            //Set transform z to transform y.
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
    }
}