using UnityEngine;

namespace Magnistart_CharacterPack
{
    public class ArrowMover : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float destroyAfterSeconds = 5f;

        private float timer = 0f;
        void Update()
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= destroyAfterSeconds) Destroy(gameObject);
        }
    }
}