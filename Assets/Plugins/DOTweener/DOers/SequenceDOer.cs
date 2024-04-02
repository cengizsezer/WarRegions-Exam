using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

	[System.Serializable]
	public class SequenceDOer : DOer
	{

		[FoldoutGroup("General")]
		public float Delay;
		[FoldoutGroup("General"), ShowIf("@this.UseCustomCurve == false")]
		public Ease Ease;
		[FoldoutGroup("General")]
		public bool UseCustomCurve;
		[FoldoutGroup("General"), ShowIf("@this.UseCustomCurve == true")]
		public AnimationCurve CustomCurve;
		[FoldoutGroup("General")]
		public int LoopCount;
		[FoldoutGroup("General"), ShowIf("@this.LoopCount != 0")]
		public LoopType LoopType;
		[FoldoutGroup("General")]
		public bool IgnoreUnityTime;
		public List<SequenceTweenData> Tweens;

		public override DGTween GetTween()
		{
			Sequence sequence = DOTween.Sequence();
			foreach (SequenceTweenData tweenData in Tweens)
			{
				switch (tweenData.TweenType)
				{
					case TweenType.Append:
						if (tweenData.Tween.UseCustomCurve)
						{
							sequence.Append(
								tweenData.Tween.GetTween(tweenData.Target).SetAs(new TweenParams()
										.SetEase(tweenData.Tween.CustomCurve)
										.SetLoops(tweenData.Tween.LoopCount, tweenData.Tween.LoopType)
										.SetDelay(tweenData.Tween.Delay)
										.SetUpdate(UpdateType.Normal, tweenData.Tween.IgnoreUnityTime))
							);
						}
						else
						{
							sequence.Append(
								tweenData.Tween.GetTween(tweenData.Target).SetAs(new TweenParams()
										.SetEase(tweenData.Tween.Ease)
										.SetLoops(tweenData.Tween.LoopCount, tweenData.Tween.LoopType)
										.SetDelay(tweenData.Tween.Delay)
										.SetUpdate(UpdateType.Normal, tweenData.Tween.IgnoreUnityTime))
							);
						}
						
						break;
					case TweenType.Insert:
						if (tweenData.Tween.UseCustomCurve)
						{
							sequence.Insert(tweenData.InsertTime, tweenData.Tween.GetTween(tweenData.Target).SetAs(new TweenParams()
								.SetEase(tweenData.Tween.CustomCurve)
								.SetLoops(tweenData.Tween.LoopCount, tweenData.Tween.LoopType)
								.SetDelay(tweenData.Tween.Delay)
								.SetUpdate(UpdateType.Normal, tweenData.Tween.IgnoreUnityTime)));
						}
						else
						{
							sequence.Insert(tweenData.InsertTime, tweenData.Tween.GetTween(tweenData.Target).SetAs(new TweenParams()
								.SetEase(tweenData.Tween.Ease)
								.SetLoops(tweenData.Tween.LoopCount, tweenData.Tween.LoopType)
								.SetDelay(tweenData.Tween.Delay)
								.SetUpdate(UpdateType.Normal, tweenData.Tween.IgnoreUnityTime)));
						}
						break;
					case TweenType.Join:
						if (tweenData.Tween.UseCustomCurve)
						{
						sequence.Join(
							tweenData.Tween.GetTween(tweenData.Target).SetAs(new TweenParams()
								.SetEase(tweenData.Tween.CustomCurve)
								.SetLoops(tweenData.Tween.LoopCount, tweenData.Tween.LoopType)
								.SetDelay(tweenData.Tween.Delay)
								.SetUpdate(UpdateType.Normal, tweenData.Tween.IgnoreUnityTime))
						);
							
						}
						else
						{
							sequence.Join(
								tweenData.Tween.GetTween(tweenData.Target).SetAs(new TweenParams()
										.SetEase(tweenData.Tween.Ease)
										.SetLoops(tweenData.Tween.LoopCount, tweenData.Tween.LoopType)
										.SetDelay(tweenData.Tween.Delay)
										.SetUpdate(UpdateType.Normal, tweenData.Tween.IgnoreUnityTime))
							);
						}
						break;
					case TweenType.AppendCallback:
						sequence.AppendCallback(() => tweenData.Callback?.Invoke());
						break;
					case TweenType.InsertCallback:
						sequence.InsertCallback(tweenData.InsertTime, () => tweenData.Callback?.Invoke());
						break;
					case TweenType.AppendInverval:
						sequence.AppendInterval(tweenData.IntervalTime);
						break;
					case TweenType.PrependInterval:
						sequence.PrependInterval(tweenData.IntervalTime);
						break;
				}
			}
			return UseCustomCurve ? 
				sequence.SetAs(new TweenParams()
					.SetEase(CustomCurve)
					.SetLoops(LoopCount, LoopType)
					.SetDelay(Delay)
					.SetUpdate(UpdateType.Normal, IgnoreUnityTime))
				.OnStart(() => OnStart?.Invoke())
				.OnComplete(() => OnComplete?.Invoke()) : 
				
				sequence.SetAs(new TweenParams()
						.SetEase(Ease)
						.SetLoops(LoopCount, LoopType)
						.SetDelay(Delay)
						.SetUpdate(UpdateType.Normal, IgnoreUnityTime))
					.OnStart(() => OnStart?.Invoke())
					.OnComplete(() => OnComplete?.Invoke());
		}

	}

	[System.Serializable]
	public class SequenceTweenData
	{

		[HideLabel, EnumToggleButtons]
		public TweenType TweenType;
		[ShowIf("IsTweenOperation"), OnValueChanged("FixTarget")]
		public Component Target;
		[ShowIf("IsTweenOperation"), InlineEditor, OnValueChanged("FixTarget")]
		public Tween Tween;
		[ShowIf("IsInsert")]
		public float InsertTime;
		[ShowIf("IsInterval")]
		public float IntervalTime;
		[ShowIf("IsCallback")]
		public UnityEvent Callback;

#if UNITY_EDITOR
		private void FixTarget()
		{
			if (Target == null || Tween == null) return;
			var targetComponent = Target.GetComponent(Tween.TargetType);
			if (targetComponent == null) return;
			Target = targetComponent;
		}
		
		private bool IsTweenOperation => TweenType == TweenType.Append || TweenType == TweenType.Insert || TweenType == TweenType.Join;
		private bool IsInsert => TweenType == TweenType.Insert || TweenType == TweenType.InsertCallback;
		private bool IsCallback => TweenType == TweenType.AppendCallback || TweenType == TweenType.InsertCallback;
		private bool IsInterval => TweenType == TweenType.PrependInterval || TweenType == TweenType.AppendInverval;
#endif
	}

}
