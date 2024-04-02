using DG.Tweening;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

    [CreateAssetMenu(menuName = "DOTweeners/Transform/LookAt", fileName = "DOLookAt")]
    public class DOLookAt : Tween
    {

        public Vector3 TargetPosition;
        public AxisConstraint AxisConstraint;
        public Vector3 Up = Vector3.up;

        public override DGTween GetTween(Component component)
        {
            Transform target = component as Transform;
            Debug.Assert(target != null, nameof(target) + " is null!");
            return target.DOLookAt(TargetPosition, Duration, AxisConstraint, Up);
        }

    }

}

