using UnityEngine;

namespace Magnistart_CharacterPack
{
    [System.Serializable]
    public class AnimationData
    {
        public string name = "Idle";
        public float crossFadeDur = 0.1f;
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterAnimationData", order = 1)]
    public class CharacterAnimationData : ScriptableObject
    {
            public AnimationData[] data = new AnimationData[] {
                new AnimationData(){name="Idle",crossFadeDur=0.15f },
                new AnimationData(){name="Run",crossFadeDur=0.15f },
                new AnimationData(){name="Attack",crossFadeDur=0.1f },
                new AnimationData(){name="Stunned",crossFadeDur=0.1f },
                new AnimationData(){name="Die",crossFadeDur=0.1f },
            };
    }
}