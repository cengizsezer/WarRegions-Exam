using DG.Tweening;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

    [CreateAssetMenu(menuName = "DOTweeners/Transform/Pather", fileName = "DOPather")]
    public class DOPather : Tween
    {

        public Vector3[] Path;
        public PathType PathType;
        public PathMode PathMode;
        public int Resolution = 10;
        public Color PathColor = Color.green;
        public bool IsLocal;

        public override DGTween GetTween(Component component)
        {
            Transform target = component as Transform;
            Debug.Assert(target != null, nameof(target) + " is null!");
            if (IsLocal)
            {
                return target.DOLocalPath(Path, Duration, PathType, PathMode, Resolution, PathColor);
            }
            else
            {
                return target.DOPath(Path, Duration, PathType, PathMode, Resolution, PathColor);
            }
        }

    }

}
