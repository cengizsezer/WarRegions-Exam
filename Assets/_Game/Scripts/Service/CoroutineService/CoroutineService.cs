using System;
using System.Collections.Generic;
using MEC;
using Zenject;

namespace MyProject.Core.Services
{
    public class CoroutineService
    {
        private CoroutineRunnerBehaviour _behaviour;

        [Inject]
        public void Construct(CoroutineRunnerBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public event Action OnPause
        {
            add => _behaviour.AddOnPauseListener(value);
            remove => _behaviour.RemoveOnPauseListener(value);
        }

        public event Action OnResume
        {
            add => _behaviour.AddOnResumeListener(value);
            remove => _behaviour.RemoveOnResumeListener(value);
        }

        public CoroutineHandle StartCoroutine(IEnumerator<float> coroutine)
        {
            return Timing.RunCoroutine(coroutine, _behaviour.gameObject);
        }

        public CoroutineHandle StartCoroutine(IEnumerator<float> coroutine, string tag)
        {
            return Timing.RunCoroutine(coroutine, _behaviour.gameObject, tag);
        }

        public void StopCoroutineWithTag(string tag)
        {
            if (!_behaviour) return;
            Timing.KillCoroutines(_behaviour.gameObject, tag);
        }

        public void StopCoroutine(CoroutineHandle coroutineHandle)
        {
            Timing.KillCoroutines(coroutineHandle);
        }

        public void StopAllCoroutines()
        {
            if (!_behaviour) return;
            Timing.KillCoroutines(_behaviour.gameObject);
        }

        public void AddOnQuitListener(Action callback)
        {
            _behaviour.AddOnQuitListener(callback);
        }
    }
}


