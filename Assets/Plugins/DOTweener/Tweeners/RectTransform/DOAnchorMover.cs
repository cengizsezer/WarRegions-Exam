using DG.Tweening;
using MyProject.General;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{


    [CreateAssetMenu(menuName = "DOTweeners/RectTransform/AnchorMover", fileName = "DOAnchorMover")]
    public class DOAnchorMover : Tween
    {

        public SelectiveVector2 TargetPosition;

        public DOType Type;
        [ShowIf("@this.Type != DOType.Default")]
        public int Vibrato = 10;
        [ShowIf("Type", DOType.Punch), PropertyRange(0, 1)]
        public int Elasticity = 1;
        [ShowIf("Type", DOType.Shake), PropertyRange(0, 180)]
        public float Randomness = 90;
        [ShowIf("Type", DOType.Shake)]
        public bool Fadeout = true;

        [HideIf("@this.Type != DOType.Default")]
        public bool From;
        [ShowIf("From")]
        public bool SetFromPosition;
        [ShowIf("@this.From && this.SetFromPosition")]
        public Vector2 FromPosition;

        public bool SnapValues = false;

        public override DGTween GetTween(Component component)
        {
            RectTransform target = component as RectTransform;
            Debug.Assert(target != null, nameof(target) + " is null!");
            Vector3 endPosition = TargetPosition.GetValue(target.anchoredPosition);
            switch (Type)
            {
                case DOType.Punch:
                    return target.DOPunchAnchorPos(endPosition, Duration, Vibrato, Elasticity, SnapValues);
                case DOType.Shake:
                    return target.DOShakeAnchorPos(Duration, endPosition, Vibrato, Randomness, SnapValues, Fadeout);
                default:
                    if (From)
                    {
                        if (SetFromPosition)
                        {
                            return target.DOAnchorPos(endPosition, Duration, SnapValues).From(FromPosition);
                        }
                        else
                        {
                            return target.DOAnchorPos(endPosition, Duration, SnapValues).From();
                        }
                    }
                    else
                    {
                        return target.DOAnchorPos(endPosition, Duration, SnapValues);
                    }
            }
        }

    }

}
