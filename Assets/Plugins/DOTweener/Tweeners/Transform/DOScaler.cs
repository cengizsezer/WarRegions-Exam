using DG.Tweening;
using MyProject.General;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

	[CreateAssetMenu(menuName = "DOTweeners/Transform/Scaler", fileName = "DOScaler")]
	public class DOScaler : Tween
	{

		public SelectiveVector3 TargetScale;
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
		public bool SetFromScale;
		[ShowIf("@this.From && this.SetFromScale")]
		public Vector3 FromScale;

		public override DGTween GetTween(Component component)
		{
			Transform target = component as Transform;
			Debug.Assert(target != null, nameof(target) + " is null!");
			Vector3 endScale = TargetScale.GetValue(target.localScale);
			switch (Type)
			{
				case DOType.Punch:
					return target.DOPunchScale(endScale, Duration, Vibrato, Elasticity);
				case DOType.Shake:
					return target.DOShakeScale(Duration, endScale, Vibrato, Randomness, Fadeout);
				default:
					if (From)
					{
						if (SetFromScale)
						{
							return target.DOScale(endScale, Duration).From(FromScale);
						}
						return target.DOScale(endScale, Duration).From();
					}
					else
					{
						return target.DOScale(endScale, Duration);
					}
			}
		}

	}

}
