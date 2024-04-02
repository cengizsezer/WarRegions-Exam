using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Services
{
    public class TimerService : IInitializable
    {
        private CoroutineService _coroutineSystem;
        private readonly List<ITimerOwner> _timers = new();
        private float _pauseTime;

        [Inject]
        public void Construct(CoroutineService coroutineSystem)
        {
            _coroutineSystem = coroutineSystem;
        }

        public void Initialize()
        {
            _coroutineSystem.StartCoroutine(TimerSystemCoroutine(), CoroutineConsts.TIMER_TAG);

            _coroutineSystem.OnPause += OnPauseConnection;
            _coroutineSystem.OnResume += OnResumeConnection;
        }

        private void OnResumeConnection()
        {
            var timePassed = Time.realtimeSinceStartup - _pauseTime;
            _pauseTime = 0;

            foreach (var timerObject in _timers.ToList())
            {
                timerObject.ResumeTime(timePassed);
            }
        }


        private void OnPauseConnection()
        {
            _pauseTime = Time.realtimeSinceStartup;
        }

        public string FormatHours(long time)
        {
            var timeSpan = TimeSpan.FromSeconds(time);
            return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }

        public void Subscribe(ITimerOwner timerObject)
        {
            if (_timers.Contains(timerObject)) return;
            _timers.Add(timerObject);
        }

        public void UnSubscribe(ITimerOwner timerObject)
        {
            if (!_timers.Contains(timerObject)) return;
            _timers.Remove(timerObject);
        }

        public void UnSubscribeAll()
        {
            _timers.Clear();
        }

        private IEnumerator<float> TimerSystemCoroutine()
        {
            do
            {
                yield return Timing.WaitForSeconds(1f);
                foreach (var timerObject in _timers.ToList())
                {
                    timerObject.UpdateTime(1f);
                }
            } while (true);
        }
    }
}


