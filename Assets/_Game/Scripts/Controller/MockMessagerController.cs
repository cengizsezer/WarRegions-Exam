using MyProject.Core.Data;
using MyProject.Core.Enums;
using MyProject.Core.Models;
using MyProject.Core.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace MyProject.Core.Controllers
{
    public class MockMessagerController : MonoBehaviour
    {
        #region Injection

        private CurrencyModel _currencyModel;
        private TaskService _taskService;
        private CurrencyController _currencyController;
        private UserController _userController;

        [Inject]
        private void Construct(CurrencyModel currencyModel,
            TaskService taskService,
            CurrencyController currencyController,
            UserController playerController)
        {
            _currencyModel = currencyModel;
            _taskService = taskService;
            _currencyController = currencyController;
            _userController = playerController;
        }

        #endregion

        #region Currency

        [SerializeField, FoldoutGroup("Currency")]
        private bool _animate;

        [SerializeField, FoldoutGroup("Currency")]
        private CurrencyData _currencyData;

        [SerializeField, FoldoutGroup("Currency")]
        private CurrencyUpdateType _currencyUpdateType;


        [Button, FoldoutGroup("Currency")]
        public void UpdateCurrency()
        {
            if (_animate && _currencyUpdateType == CurrencyUpdateType.Gain)
            {
                CurrencyGainAnimationGameTask gameTask = new CurrencyGainAnimationGameTask();
                _currencyController.AddCurrency(_currencyData);
                Debug.Log(_currencyData.CurrencyType);
                gameTask.Initialize(_currencyData, Vector2.zero);
                _taskService.AddTask(gameTask);


            }
            else
            {
                _currencyController.UpdateCurrency(_currencyData);
                UpdateCurrencyGameTask gameTask = new UpdateCurrencyGameTask();
                gameTask.Initialize(_currencyData, _currencyUpdateType);
                _taskService.AddTask(gameTask);
            }
        }

        [Button, FoldoutGroup("Currency")]
        public void ResetCurrency(CurrencyType currencyType)
        {
            UpdateCurrencyGameTask gameTask = new UpdateCurrencyGameTask();

            CurrencyData currencyData;

            switch (currencyType)
            {
                case CurrencyType.Coin:
                    currencyData = new CurrencyData { CurrencyType = CurrencyType.Coin, CurrencyValue = _currencyModel.CoinValue };
                    break;
                case CurrencyType.Mana:
                    currencyData = new CurrencyData { CurrencyType = CurrencyType.Mana, CurrencyValue = _currencyModel.ManaValue };
                    break;
                default:
                    currencyData = new CurrencyData();
                    break;
            }

            _currencyController.TryConsume(currencyData);

            gameTask.Initialize(currencyData, CurrencyUpdateType.Consume);
            _taskService.AddTask(gameTask);
        }

        #endregion

        #region GameTasks


        [Button, FoldoutGroup("GameTasks")]
        private void LockUIGameTask()
        {
            _taskService.AddTask(new LockUIGameTask());
        }

        [Button, FoldoutGroup("GameTasks")]
        private void UnlockUIGameTask()
        {
            _taskService.AddTask(new UnlockUIGameTask());
        }

        #endregion

        #region Level

        [Button, FoldoutGroup("LevelWin")]
        private void LevelWin()
        {
            _userController.LevelWin();
        }

        [Button, FoldoutGroup("LevelFail")]
        private void LevelFail()
        {
            _userController.LevelFail();
        }


        #endregion
    }
}

