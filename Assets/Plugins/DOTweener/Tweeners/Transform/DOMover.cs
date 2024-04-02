using DG.Tweening;
using MyProject.General;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

	[CreateAssetMenu(menuName = "DOTweeners/Transform/Mover", fileName = "DOMover")]
	public class DOMover : Tween
	{

		public SelectiveVector3 TargetPosition;

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
		public Vector3 FromPosition;
		[HideIf("@this.Type != DOType.Default")]
		public bool IsLocal;
		public bool SnapValues;

		public override DGTween GetTween(Component component)
		{
			Transform target = component as Transform;
			Debug.Assert(target != null, nameof(target) + " is null!");
			Vector3 endPosition = TargetPosition.GetValue(IsLocal ? target.localPosition : target.position);
			switch (Type)
			{
				case DOType.Punch:
					return target.DOPunchPosition(endPosition, Duration, Vibrato, Elasticity, SnapValues);
				case DOType.Shake:
					return target.DOShakePosition(Duration, endPosition, Vibrato, Randomness, SnapValues, Fadeout);
				default:
					if (IsLocal)
					{
						if (From)
						{
							if (SetFromPosition)
							{
								return target.DOLocalMove(endPosition, Duration, SnapValues).From(FromPosition);
							}
							return target.DOLocalMove(endPosition, Duration, SnapValues).From();
						}
						return target.DOLocalMove(endPosition, Duration, SnapValues);
					}
					else
					{
						if (From)
						{
							if (SetFromPosition)
							{
								return target.DOMove(endPosition, Duration, SnapValues).From(FromPosition);
							}
							return target.DOMove(endPosition, Duration, SnapValues).From();
						}
						return target.DOMove(endPosition, Duration, SnapValues);
					}
			}
		}

	}

}
