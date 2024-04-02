using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{
	[CreateAssetMenu(menuName = "DOTweeners/CanvasGroup/Fader", fileName = "DOCanvasGroupFader")]
	public class DOCanvasGroupFader : Tween
	{
		public float TargetAlpha;
		public bool From;

		[ShowIf("From")]
		public bool SetFromAlpha;

		[ShowIf("@this.From && this.SetFromAlpha")]
		public float FromAlpha;

		public override DGTween GetTween(Component component)
		{
			CanvasGroup target = component.gameObject.GetComponent<CanvasGroup>();
			Debug.Assert(target != null, nameof(target) + " is null!");
			if (From)
			{
				if (SetFromAlpha)
				{
					return target.DOFade(TargetAlpha, Duration).From(FromAlpha);
				}

				return target.DOFade(TargetAlpha, Duration).From();
			}

			return target.DOFade(TargetAlpha, Duration);
		}
	}
}