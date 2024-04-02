using System.Collections.Generic;
using MyProject.Core.Services;
using MEC;
using Zenject;

namespace MyProject.Core.View
{
    public class TransitionView : BaseView
    {
        private CoroutineHandle _coroutineHandle;

        #region Injection

       
        private CoroutineService _coroutineService;

        [Inject]
        private void Construct(
            CoroutineService coroutineService)
        {
           
            _coroutineService = coroutineService;
        }

        #endregion

        public override void Initialize()
        {
            _signalBus.Subscribe<StartTransitionSignal>(OnTransitionStart);
            
        }

        private void OnTransitionStart(StartTransitionSignal signal)
        {
            _coroutineHandle = _coroutineService.StartCoroutine(SetActiveWithDelay(signal.Duration));
        }

        private IEnumerator<float> SetActiveWithDelay(float duration) 
        {
            gameObject.SetActive(true);
            yield return Timing.WaitForSeconds(duration);
            gameObject.SetActive(false);
        }

        public override void Dispose()
        {
            if (_coroutineHandle.IsRunning)
            {
                _coroutineService.StopCoroutine(_coroutineHandle);
                if (gameObject) gameObject.SetActive(false);
            }
            _signalBus.Unsubscribe<StartTransitionSignal>(OnTransitionStart);
        }
    }
}