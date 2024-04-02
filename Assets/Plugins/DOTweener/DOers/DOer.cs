using Sirenix.OdinInspector;
using UnityEngine.Events;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

    [System.Serializable]
    public abstract class DOer
    {

        public string Name = "NewTween";
        [FoldoutGroup("Play Options"), HideLabel, PropertyOrder(100), EnumToggleButtons]
        public PlayOptions PlayOptions = PlayOptions.PlayOnEnabled;
        [FoldoutGroup("Kill Options"), HideLabel, PropertyOrder(100), EnumToggleButtons]
        public KillOptions KillOptions = KillOptions.KillDontComplete;
        [FoldoutGroup("Events"), PropertyOrder(100)]
        public UnityEvent OnStart;
        [FoldoutGroup("Events"), PropertyOrder(100)]
        public UnityEvent OnComplete;

        public abstract DGTween GetTween();

    }

}
