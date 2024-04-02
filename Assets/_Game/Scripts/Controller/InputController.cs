using MyProject.Core.Controllers;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MyProject.Core.Controllers
{
    public class InputController : BaseController
    {
        public Vector3 FingerPosition => LeanTouch.Fingers[0].ScreenPosition;

        #region Injection
        private SignalBus _signalBus;
        #endregion

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        protected override void OnInitialize()
        {
            LeanTouch.OnFingerDown += OnInputFingerDown;
            LeanTouch.OnFingerUp += OnFingerUp;
            LeanTouch.OnFingerUpdate += OnFingerUpdate;
        }

        private void OnInputFingerDown(LeanFinger finger)
        {
            _signalBus.Fire(new FingerDownSignal(finger));
        }

        private void OnFingerUpdate(LeanFinger finger)
        {
            _signalBus.Fire(new FingerUpdateSignal(finger));

#if UNITY_EDITOR
            if (LeanTouch.Fingers.Count > 2)
            {
                float pinchValue = LeanGesture.GetPinchScale();
                _signalBus.Fire(new FingerPinchSignal(pinchValue));
            }
#else
             if (LeanTouch.Fingers.Count > 1)
             {
                float pinchValue = LeanGesture.GetPinchScale();
                _signalBus.Fire(new FingerPinchSignal(pinchValue));
             }
#endif
        }

        private void OnFingerUp(LeanFinger finger)
        {
            _signalBus.Fire(new FingerUpSignal(finger));
        }

        protected override void OnApplicationReadyToStart()
        {
        }

        protected override void OnDispose()
        {
            LeanTouch.OnFingerDown -= OnInputFingerDown;
            LeanTouch.OnFingerUp -= OnFingerUp;
            LeanTouch.OnFingerUpdate -= OnFingerUpdate;
        }
    }
}

