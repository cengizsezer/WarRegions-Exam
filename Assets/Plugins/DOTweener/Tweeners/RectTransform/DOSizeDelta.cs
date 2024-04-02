using DG.Tweening;
using MyProject.General;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

    [CreateAssetMenu(menuName = "DOTweeners/RectTransform/SizeDelta", fileName = "DOSizeDelta")]
    public class DOSizeDelta : Tween
    {
        public SelectiveVector2 TargetSizeDelta;

        public bool From;
        [ShowIf("From")]
        public bool SetFromSizeDelta;
        [ShowIf("@this.From && this.SetFromSizeDelta")]
        public Vector2 FromSizeDelta;

        public bool SnapValues = false;


        public override DGTween GetTween(Component component)
        {
            RectTransform target = component as RectTransform;
            Debug.Assert(target != null, nameof(target) + " is null!");
            Vector3 endSizeDelta = TargetSizeDelta.GetValue(target.sizeDelta);
            if (From)
            {
                if (SetFromSizeDelta)
                {
                    return target.DOSizeDelta(endSizeDelta, Duration, SnapValues).From(FromSizeDelta);
                }
                else
                {
                    return target.DOSizeDelta(endSizeDelta, Duration, SnapValues).From();
                }
            }
            else
            {
                return target.DOSizeDelta(endSizeDelta, Duration, SnapValues);
            }
        }

    }

}
