using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using DGTween = DG.Tweening.Tween;
using UnityEngine.UI;

namespace MyProject.Tween
{
	[CreateAssetMenu(menuName = "DOTweeners/TextMeshPro/DOTextMeshProUGUIColor", fileName = "DOTextMeshProUGUIColor")]
	public class DOTextMeshProUGUIColor : Tween
	{
		public Color TargetColor;
		public bool From;

		[ShowIf("From")]
		public bool SetFromColor;

		[ShowIf("@this.From && this.SetFromColor")]
		public Color FromColor;

		public override DGTween GetTween(Component component)
		{
			TextMeshProUGUI target = component as TextMeshProUGUI;
			Debug.Assert(target != null, $"{nameof(DOTextMeshProUGUIColor)} tween expects {nameof(TextMeshProUGUI)} component attached as target. Make sure you attach the component instead of {component.GetType()}");
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
	}
}