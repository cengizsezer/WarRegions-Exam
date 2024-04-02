using DG.Tweening;
using MyProject.General;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

	[CreateAssetMenu(menuName = "DOTweeners/Transform/Jumper", fileName = "DOJumper")]
	public class DOJumper : Tween
	{

		public SelectiveVector3 EndPosition;
		public float JumpPower;
		public int JumpCount;
		public bool SnapValues;
		public bool IsLocal;

		public override DGTween GetTween(Component component)
		{
			Transform target = component as Transform;
			Debug.Assert(target != null, nameof(target) + " is null!");
			Vector3 endPosition = EndPosition.GetValue(target.position);
			if (IsLocal)
			{
				return target.DOLocalJump(endPosition, JumpPower, JumpCount, Duration, SnapValues);
			}
			return target.DOJump(endPosition, JumpPower, JumpCount, Duration, SnapValues);
		}

	}

}
