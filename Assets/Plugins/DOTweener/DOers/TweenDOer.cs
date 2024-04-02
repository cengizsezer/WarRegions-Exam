using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

	[System.Serializable]
	public class TweenDOer : DOer
	{

		[OnValueChanged("FixTarget")]
		public Component Target;
		[InlineEditor, OnValueChanged("FixTarget")]
		public Tween Tween;

		public override DGTween GetTween()
		{
			return Tween.UseCustomCurve ?  
				
				Tween.GetTween(Target)
				.SetAs(new TweenParams()
					.SetEase(Tween.CustomCurve)
					.SetLoops(Tween.LoopCount, Tween.LoopType)
					.SetDelay(Tween.Delay)
					.SetUpdate(UpdateType.Normal, Tween.IgnoreUnityTime))
				.OnStart(() => OnStart?.Invoke())
				.OnComplete(() => OnComplete?.Invoke()) :
				
				Tween.GetTween(Target)
				.SetAs(new TweenParams()
					.SetEase(Tween.Ease)
					.SetLoops(Tween.LoopCount, Tween.LoopType)
					.SetDelay(Tween.Delay)
					.SetUpdate(UpdateType.Normal, Tween.IgnoreUnityTime))
				.OnStart(() => OnStart?.Invoke())
				.OnComplete(() => OnComplete?.Invoke());
		}

#if UNITY_EDITOR
		private void FixTarget()
		{
			if (Target == null || Tween == null) return;
			var targetComponent = Target.GetComponent(Tween.TargetType);
			if (targetComponent == null) return;
			Target = targetComponent;
		}
#endif

	}

}