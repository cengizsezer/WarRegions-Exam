using System.Collections.Generic;
using System.Linq;
using MyProject.Core.Enums;
using MEC;
using UnityEngine;
using Zenject;

namespace MyProject.Core.Services
{
    public class FlagService
    {
        private readonly Dictionary<string, FlagState> _flags = new();

        #region Injection
        private SignalBus _signalBus;
        private CoroutineService _coroutineService;

        [Inject]
        public void Construct(SignalBus signalBus
            , CoroutineService coroutineService)
        {
            _signalBus = signalBus;
            _coroutineService = coroutineService;
        }
        #endregion

        public void SetFlag(string flagName, FlagState flagState, float flagTime = 0f)
        {
            if (!IsFlagAvailable(flagName) && flagState != FlagState.Available)
            {
                return;
            }

            _flags[flagName] = flagState;

            
            _signalBus.Fire(new FlagStatusChangedSignal
            {
                FlagName = flagName
            });

            if (flagState == FlagState.Unavailable && flagTime > 0f)
            {
                _coroutineService.StartCoroutine(FlagTimeCoroutine(flagName, flagTime), flagName);
            }

            DebugLogger.Log($"{flagName} flag is set to {flagState}", this, new Color(0.5f, 0.5f, 0f));
        }

        private IEnumerator<float> FlagTimeCoroutine(string flagName, float flagTime)
        {
            _flags[flagName] = FlagState.WaitingToBeAvailable;

            yield return Timing.WaitForSeconds(flagTime);

            _flags[flagName] = FlagState.Available;

            _signalBus.Fire(new FlagStatusChangedSignal
            {
                FlagName = flagName
            });

        }

        public FlagState GetFlag(string flagName)
        {
            var keyExists = _flags.TryGetValue(flagName, out _);

            if (!keyExists)
            {
                _flags[flagName] = FlagState.Available;
            }

            return _flags[flagName];
        }

        public bool IsFlagAvailable(string flagName)
        {
            var keyExists = _flags.TryGetValue(flagName, out _);

            if (!keyExists)
            {
                _flags[flagName] = FlagState.Available;
            }

            return _flags[flagName] == FlagState.Available;
        }

        public void ClearFlags()
        {
            foreach (var flag in _flags.ToList())
            {
                _flags[flag.Key] = FlagState.Available;
            }
        }
    }
}



