using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;
using UnityEngine.UI;


namespace MyProject.Tween
{
	[CreateAssetMenu(menuName = "DOTweeners/Image/DOColor", fileName = "DOImageColor")]
	public class DOImageColor : Tween
	{
		public Color TargetColor;
		public bool From;

		[ShowIf("From")]
		public bool SetFromColor;

		[ShowIf("@this.From && this.SetFromColor")]
		public Color FromColor;

		public override DGTween GetTween(Component component)
		{
			Image target = component as Image;
			Debug.Assert(target != null, nameof(target) + " is null!");
			if (From)
			{
				if (SetFromColor)
				{
					return target.DOColor(TargetColor, Duration).From(FromColor);
				}

				return target.DOColor(TargetColor, Duration).From();
			}

			return target.DOColor(TargetColor, Duration);
		}
		
#if UNITY_EDITOR
		public override Type TargetType => typeof(Image);
#endif
		
	}
}