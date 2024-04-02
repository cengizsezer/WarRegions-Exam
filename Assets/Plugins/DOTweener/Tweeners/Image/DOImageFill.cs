using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.Tween
{
	[CreateAssetMenu(menuName = "DOTweeners/Image/DOFill", fileName = nameof(DOImageFill))]
	public class DOImageFill : Tween
	{

		public float TargetFill;
		public bool From;
		[ShowIf("From")]
		public bool SetFromFill;
		[ShowIf("@this.From && this.SetFromFill")]
		public float FromFill;
		
		public override DG.Tweening.Tween GetTween(Component component)
		{
			Image target = component as Image;
			Debug.Assert(target != null, nameof(target) + " is null!");
			if (From)
			{
				if (SetFromFill)
				{
					return target.DOFillAmount(TargetFill, Duration).From(FromFill);
				}
				return target.DOFillAmount(TargetFill, Duration).From();
			}
			return target.DOFillAmount(TargetFill, Duration);
		}

#if UNITY_EDITOR
		public override Type TargetType => typeof(Image);
#endif

	}
}
