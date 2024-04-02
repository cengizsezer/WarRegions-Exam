using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

    public abstract class Tween : ScriptableObject
    {

        [FoldoutGroup("General")]
        public float Duration;
        [FoldoutGroup("General")]
        public float Delay;
        [FoldoutGroup("General"), ShowIf("@this.UseCustomCurve == false")]
        public Ease Ease;
        [FoldoutGroup("General")]
        public bool UseCustomCurve;
        [FoldoutGroup("General"), ShowIf("@this.UseCustomCurve == true")]
        public AnimationCurve CustomCurve;
        [FoldoutGroup("General")]
        public int LoopCount;
        [FoldoutGroup("General"), ShowIf("@this.LoopCount != 0")]
        public LoopType LoopType;

        [FoldoutGroup("General")]
        public bool IgnoreUnityTime;
        public abstract DGTween GetTween(Component component);

#if UNITY_EDITOR
        public virtual Type TargetType => typeof(Component);
#endif

    }

}