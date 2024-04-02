using UnityEngine;

namespace Magnistart_CharacterPack
{
    public class ParticleAutoDestroyer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSys = null;
        private void Awake()
        {
            particleSys.Play();
            Destroy(gameObject, particleSys.main.startDelay.constant + particleSys.main.startLifetime.constant);
        }
        private void Reset()
        {
            particleSys = GetComponent<ParticleSystem>();
        }
    }
}