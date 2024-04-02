using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using MyProject.Core.Services;
using MyProject.Core.Enums;
using MyProject.Core.Settings;

namespace MyProject.Core.Services
{
    public class PopupService : IInitializable, IDisposable
    {
        private BasePopupParameters _popupParameters;
        private IBasePopup _popup;
        private readonly Queue<BasePopupParameters> _queue = new();
        private BasePopupParameters _normalParams, _immediateParams;
        private IBasePopup _normalPopup, _immediatePopup;

        #region Injection

        private DiContainer _diContainer;
        private PopupSettings _settings;
        private TaskService _taskService;
        private Transform _uiParent;

        [Inject]
        public void Construct(DiContainer diContainer
            , PopupSettings settings
            , TaskService taskService
            , [Inject(Id = "uiParent")] Transform uiParent)
        {
            _diContainer = diContainer;
            _settings = settings;
            _taskService = taskService;
            _uiParent = uiParent;
        }

        #endregion

        public void Initialize()
        {
        }

        public void AddPopup(BasePopupParameters parameters)
        {
            if (parameters.PopupPriority == PopupPriority.IMMEDIATE)
            {
                ShowImmediatePopup(parameters);
                return;
            }
           
            PopupGameTask popupTask = new PopupGameTask();
            Assert.IsTrue(parameters.Task == null, "The task will be automatically handled in PopupService. Do not assign task outside.");
            parameters.Task = popupTask;
            popupTask.Initialize(parameters);
            _taskService.AddTask(popupTask);
        }

        public string GetOpenedPopupName()
        {
            if (_immediateParams != null)
            {
                return _immediateParams.PopupName();
            }

            if (_normalParams != null)
            {
                return _normalParams.PopupName();
            }

            return "";
        }

        public BasePopupParameters GetOpenedPopupParameters()
        {
            return _immediateParams ?? _normalParams;
        }

        public IBasePopup GetOpenedPopup()
        {
            return _immediatePopup != null ? _immediatePopup : _normalPopup;
        }


        public void AddPopupAtFirst(BasePopupParameters parameters)
        {
            if (parameters.PopupPriority == PopupPriority.IMMEDIATE)
            {
                return;
            }

            PopupGameTask popupTask = new PopupGameTask();
            Assert.IsTrue(parameters.Task == null, "The task will be automatically handled in PopupService. Do not assign task outside.");
            parameters.Task = popupTask;
            popupTask.Initialize(parameters);
            _taskService.AddTask(popupTask, true);

        }

        public void ShowPopup(BasePopupParameters parameters)
        {
            ShowNormalPopup(parameters);
        }

        public void ShowImmediatePopup(BasePopupParameters parameters)
        {
            RemoveImmediatePopup();
            var popupPrefab = _settings.GetPopupByName(parameters.PopupName());
            Assert.IsTrue(popupPrefab != null, $"Couldn't find Popup {parameters.PopupName()} in PopupSettings. Please make sure that you add this popup to PopupSettings.");
            if (popupPrefab == null) return;

            var go = _diContainer.InstantiatePrefab(popupPrefab, _uiParent);
            EnablePopupCamera(true);

            _immediateParams = parameters;
            _immediatePopup = go.GetComponent<IBasePopup>();
            _immediatePopup.Init(_immediateParams);
            _immediatePopup.Show();
        }

        private void ShowNormalPopup(BasePopupParameters parameters)
        {
            var popupPrefab = _settings.GetPopupByName(parameters.PopupName());
            Assert.IsTrue(popupPrefab != null, $"Couldn't find Popup {parameters.PopupName()} in PopupSettings. Please make sure that you add this popup to PopupSettings.");
            if (popupPrefab == null)
            {
                RemoveNormalPopup(parameters);
                return;
            }

            var go = _diContainer.InstantiatePrefab(popupPrefab, _uiParent);
            EnablePopupCamera(true);

            _normalParams = parameters;
            _normalPopup = go.GetComponent<IBasePopup>();
            _normalPopup.Init(_normalParams);
            _normalPopup.Show();
        }


        private void ImmediatePopupBundleLoaded(List<GameObject> result, List<AssetBundle> bundles)
        {
            var go = _diContainer.InstantiatePrefab(result[0]);

            _immediatePopup = go.GetComponent<IBasePopup>();
            EnablePopupCamera(true);

            _immediatePopup.Init(_immediateParams);
            _immediatePopup.Show();
        }

        private void NormalPopupBundleLoaded(List<GameObject> result, List<AssetBundle> bundles)
        {
            var go = _diContainer.InstantiatePrefab(result[0]);
            EnablePopupCamera(true);
            _normalPopup = go.GetComponent<IBasePopup>();
            _normalPopup.Init(_normalParams);
            _normalPopup.Show();

            //_assetBundleLoadService.UnloadAssetBundles(bundles);
        }

        private void EnablePopupCamera(bool enabled)
        {
            if (_normalPopup != null || _immediatePopup != null && !enabled) //if either of popups not null and we are trying to disable the camera, prevent disabling it.
            {
                return;
            }
        }

        /// <summary>
        /// This function should not be accessed out of Popups. 
        /// </summary>
        /// <param name="parameters"></param>
        public void RemoveNormalPopup(BasePopupParameters parameters)
        {
            if (_normalParams == parameters)
            {
                IBasePopup popupToDestroy = _normalPopup;
                _normalPopup = null;
                _normalParams = null;
                popupToDestroy.DestroyPopup(() => EnablePopupCamera(false));
            }
            else if (parameters.Task != null)
            {
                _taskService.RemoveUnprocessedTask(parameters.Task);
                parameters.Task.CompleteTask();
            }
        }

        public void RemoveImmediatePopup()
        {
            if (_immediatePopup != null)
            {
                if (_normalPopup != null)
                {
                    _immediatePopup.DestroyPopup();
                }
                else
                {
                    _immediatePopup.DestroyPopup(() => EnablePopupCamera(false));
                }

                _immediatePopup = null;
                _immediateParams = null;
            }
        }

        public void DestroyAllPopups()
        {
            RemoveImmediatePopup();

            if (_normalPopup != null)
            {
                IBasePopup popupToDestroy = _normalPopup;
                _normalPopup = null;
                _normalParams = null;
                popupToDestroy.DestroyPopup(() => EnablePopupCamera(false));
            }
        }

        public void Dispose()
        {
        }

        #region Editor

#if UNITY_EDITOR
        public IBasePopup CurrentPopup => _popup;
#endif

        #endregion
    }
}


