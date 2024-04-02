using DG.Tweening;
using MyProject.General;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

    [CreateAssetMenu(menuName = "DOTweeners/Transform/Rotator", fileName = "DORotator")]
    public class DORotator : Tween
    {

        public SelectiveVector3 TargetRotation;

        public DOType Type;
        [ShowIf("@this.Type != DOType.Default")]
        public int Vibrato = 10;
        [ShowIf("Type", DOType.Punch), PropertyRange(0, 1)]
        public int Elasticity = 1;
        [ShowIf("Type", DOType.Shake), PropertyRange(0, 180)]
        public float Randomness = 90;
        [ShowIf("Type", DOType.Shake)]
        public bool Fadeout = true;

        public RotateMode RotateMode;
        [HideIf("@this.Type != DOType.Default")]
        public bool From;
        [ShowIf("From")]
        public bool SetFromRotation;
        [ShowIf("@this.From && this.SetFromRotation")]
        public Vector3 FromRotation;
        [HideIf("@this.Type != DOType.Default")]
        public bool IsLocal = false;

        public override DGTween GetTween(Component component)
        {
            Transform target = component as Transform;
            Debug.Assert(target != null, nameof(target) + " is null!");
            Vector3 endRotation = TargetRotation.GetValue(IsLocal ? target.localRotation.eulerAngles : target.rotation.eulerAngles);
            switch (Type)
            {

                case DOType.Punch:
                    return target.DOPunchRotation(endRotation, Duration, Vibrato, Elasticity);
                case DOType.Shake:
                    return target.DOShakeRotation(Duration, endRotation, Vibrato, Randomness, Fadeout);
                default:
                    if (IsLocal)
                    {
                        if (From)
                        {
                            if (SetFromRotation)
                            {
                                return target.DOLocalRotate(endRotation, Duration, RotateMode).From(FromRotation);
                            }
                            else
                            {
                                return target.DOLocalRotate(endRotation, Duration, RotateMode).From();
                            }
                        }
                        else
                        {
                            return target.DOLocalRotate(endRotation, Duration, RotateMode);
                        }
                    }
                    else
                    {
                        if (From)
                        {
                            if (SetFromRotation)
                            {
                                return target.DORotate(endRotation, Duration, RotateMode).From(FromRotation);
                            }
                            else
                            {
                                return target.DORotate(endRotation, Duration, RotateMode).From();
                            }
                        }
                        else
                        {
                            return target.DORotate(endRotation, Duration, RotateMode);
                        }
                    }
            }
        }

    }

}
