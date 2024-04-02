using System;
using UnityEngine;

namespace MyProject.Core.Services
{
    public class CoroutineRunnerBehaviour : MonoBehaviour
    {
        public event Action OnQuit;
        public event Action OnPause;
        public event Action OnResume;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                OnPause?.Invoke();
            }
            else
            {
                OnResume?.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            OnQuit?.Invoke();
            OnQuit = null;
        }

        public void AddOnQuitListener(Action action)
        {
            if (OnQuit != null && Array.Exists(OnQuit.GetInvocationList(), (a) => a.Equals(action)))
            {
                return;
            }
            OnQuit += action;
        }

        public void AddOnPauseListener(Action action)
        {
            if (OnPause != null && Array.Exists(OnPause.GetInvocationList(), (a) => a.Equals(action)))
            {
                return;
            }
            OnPause += action;
        }

        public void RemoveOnPauseListener(Action action)
        {
            if (OnPause != null && Array.Exists(OnPause.GetInvocationList(), (a) => a.Equals(action)))
            {
                OnPause -= action;
            }
        }

        public void AddOnResumeListener(Action action)
        {
            if (OnResume != null && Array.Exists(OnResume.GetInvocationList(), (a) => a.Equals(action)))
            {
                return;
            }
            OnResume += action;
        }

        public void RemoveOnResumeListener(Action action)
        {
            if (OnResume != null && Array.Exists(OnResume.GetInvocationList(), (a) => a.Equals(action)))
            {
                OnResume -= action;
            }
        }
    }
}


