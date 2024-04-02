using UnityEngine;

namespace Magnistart_CharacterPack
{
    public class ThrowingAxe : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private Sprite rotatingSprite = null;
        [SerializeField] private GameObject trail = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float destroyAfterSeconds = 5f;
        private bool isShot = false;
        private float timer = 0f;

        public void Shoot()
        {
            spriteRenderer.sprite = rotatingSprite;
            trail.SetActive(true);
            animator.enabled = true;
            isShot = true;
        }
        void Update()
        {
            if (!isShot) return;
            transform.position += Vector3.right * speed * Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= destroyAfterSeconds) Destroy(gameObject);
        }
    }
}