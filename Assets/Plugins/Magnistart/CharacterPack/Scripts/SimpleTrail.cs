using UnityEngine;

namespace Magnistart_CharacterPack
{
    public class SimpleTrail : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr = null;
        private Vector2 startPos = Vector2.zero;
        public float maxSize = 3f;
        private void OnEnable()
        {
            startPos = transform.position;
            sr.size = new Vector2(0f, sr.size.y);
        }
        private void Update()
        {
            //Stretch trail up to maximum size.
            sr.size = new Vector2(Mathf.Min(((Vector2)transform.position - startPos).magnitude, maxSize), sr.size.y);
        }
        private void Reset()
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }
}