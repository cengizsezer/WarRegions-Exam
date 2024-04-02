using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using DGTween = DG.Tweening.Tween;

namespace MyProject.Tween
{

	public class DOTweener : MonoBehaviour
	{

		[LabelText("Tween DOers"), ListDrawerSettings(ListElementLabelName = "Name")]
		public List<TweenDOer> TweenDOers;
		[LabelText("Sequence DOers")]
		public List<SequenceDOer> SequenceDOers;
		private readonly Dictionary<string, DGTween> playedTweens = new Dictionary<string, DGTween>();

		#region Unity
		private void Awake()
		{
			foreach (TweenDOer tweenDOer in TweenDOers
				.Where(x => x.PlayOptions == PlayOptions.PlayOnAwake))
			{
				PlayNamed(tweenDOer.Name);
			}
			foreach (SequenceDOer sequenceDOer in SequenceDOers
				.Where(x => x.PlayOptions == PlayOptions.PlayOnAwake))
			{
				PlayNamed(sequenceDOer.Name);
			}
		}

		private void OnEnable()
		{
			foreach (TweenDOer tweenDOer in TweenDOers
				.Where(x => x.PlayOptions == PlayOptions.PlayOnEnabled))
			{
				PlayNamed(tweenDOer.Name);
			}
			foreach (SequenceDOer sequenceDOer in SequenceDOers
				.Where(x => x.PlayOptions == PlayOptions.PlayOnEnabled))
			{
				PlayNamed(sequenceDOer.Name);
			}
		}

		private void OnDisable()
		{
			foreach (TweenDOer tweenDOer in TweenDOers)
			{
				switch (tweenDOer.KillOptions)
				{
					case KillOptions.KillComplete:
						KillNamed(true, tweenDOer.Name);
						break;
					case KillOptions.CompleteWithCallbacks:
						CompleteNamed(true, tweenDOer.Name);
						break;
					case KillOptions.CompleteWithoutCallbacks:
						CompleteNamed(false, tweenDOer.Name);
						break;
					case KillOptions.KillDontComplete:
						KillNamed(false, tweenDOer.Name);
						break;
				}
			}
			foreach (SequenceDOer sequenceDOer in SequenceDOers)
			{
				switch (sequenceDOer.KillOptions)
				{
					case KillOptions.KillComplete:
						KillNamed(true, sequenceDOer.Name);
						break;
					case KillOptions.CompleteWithCallbacks:
						CompleteNamed(true, sequenceDOer.Name);
						break;
					case KillOptions.CompleteWithoutCallbacks:
						CompleteNamed(false, sequenceDOer.Name);
						break;
					case KillOptions.KillDontComplete:
						KillNamed(false, sequenceDOer.Name);
						break;
				}
			}
		}

		private void Start()
		{
			foreach (TweenDOer tweenDOer in TweenDOers
				.Where(x => x.PlayOptions == PlayOptions.PlayOnStart))
			{
				PlayNamed(tweenDOer.Name);
			}
			foreach (SequenceDOer sequenceDOer in SequenceDOers
				.Where(x => x.PlayOptions == PlayOptions.PlayOnStart))
			{
				PlayNamed(sequenceDOer.Name);
			}
		}
		#endregion

		#region Play
		public void PlayAll()
		{
			KillAll();
			foreach (TweenDOer tweenDoer in TweenDOers)
			{
				playedTweens.Add(tweenDoer.Name, tweenDoer.GetTween());
			}
			foreach (SequenceDOer sequenceDOer in SequenceDOers)
			{
				playedTweens.Add(sequenceDOer.Name, sequenceDOer.GetTween());
			}
		}

		public void PlaySingleNamed(string tweenName) // Used to serialize to unity editor
		{
			PlayNamed(tweenName);
		}

		public void PlayNamed(params string[] tweenNames)
		{
			KillNamed(false, tweenNames);
			foreach (string tweenName in tweenNames)
			{
				TweenDOer tweenDOer = TweenDOers.FirstOrDefault(x => x.Name == tweenName);
				if (tweenDOer != null)
				{
					playedTweens.Add(tweenDOer.Name, tweenDOer.GetTween());
				}
				SequenceDOer sequenceDOer = SequenceDOers.FirstOrDefault(x => x.Name == tweenName);
				if (sequenceDOer != null)
				{
					playedTweens.Add(sequenceDOer.Name, sequenceDOer.GetTween());
				}
			}
		}
		#endregion

		#region Kill
		public void KillAll(bool complete = false)
		{
			foreach (DGTween tween in playedTweens.Values)
			{
				tween.Kill(complete);
			}
			playedTweens.Clear();
		}

		public void KillNamed(bool complete, params string[] tweenNames)
		{
			foreach (string tweenName in tweenNames)
			{
				if (playedTweens.ContainsKey(tweenName))
				{
					playedTweens[tweenName].Kill(complete);
					playedTweens.Remove(tweenName);
				}
			}
		}
		#endregion

		#region Complete
		public void CompleteAll(bool withCallbacks = false)
		{
			foreach (DGTween tween in playedTweens.Values)
			{
				if (tween is Sequence)
				{
					tween.Complete(withCallbacks);
				}
				else
				{
					tween.Complete();
				}
			}
			playedTweens.Clear();
		}

		public void CompleteNamed(bool withCallbacks, params string[] tweenNames)
		{
			foreach (string tweenName in tweenNames)
			{
				if (playedTweens.ContainsKey(tweenName))
				{
					DGTween tween = playedTweens[tweenName];
					if (tween is Sequence)
					{
						tween.Complete(withCallbacks);
					}
					else
					{
						tween.Complete();
					}
					playedTweens.Remove(tweenName);
				}
			}
		}
		#endregion

		#region Editor
#if UNITY_EDITOR
		[Button("Assign Self", ButtonSizes.Medium)]
		private void AssignSelf()
		{
			foreach (TweenDOer tweenDOer in TweenDOers)
			{
				tweenDOer.Target = transform;
			}
			foreach (SequenceDOer sequenceDOer in SequenceDOers)
			{
				foreach (SequenceTweenData tweenData in sequenceDOer.Tweens)
				{
					tweenData.Target = transform;
				}
			}
		}

		[Button(ButtonSizes.Medium), HideInEditorMode]
		private void Play(params string[] tweenNames)
		{
			if (tweenNames == null || tweenNames.Length == 0)
			{
				PlayAll();
			}
			else
			{
				PlayNamed(tweenNames);
			}
		}

		[Button(ButtonSizes.Medium), HideInEditorMode]
		private void Kill(bool complete, params string[] tweenNames)
		{
			if (tweenNames == null || tweenNames.Length == 0)
			{
				KillAll(complete);
			}
			else
			{
				KillNamed(complete, tweenNames);
			}
		}

		[Button(ButtonSizes.Medium), HideInEditorMode]
		private void Complete(bool withCallbacks, params string[] tweenNames)
		{
			if (tweenNames == null || tweenNames.Length == 0)
			{
				CompleteAll(withCallbacks);
			}
			else
			{
				CompleteNamed(withCallbacks, tweenNames);
			}
		}

#endif
		#endregion

	}

}
