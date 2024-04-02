using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Zenject;

namespace MyProject.Core.Services
{
    public delegate void TaskCompleteCallback(float delay = 0f);

    public class TaskService : IInitializable, IDisposable
    {
        private readonly LinkedList<ITask> _tasks = new LinkedList<ITask>();

        private bool _isRunning;
        private bool _isPaused;

        private CancellationTokenSource _cancellationToken;

        private DiContainer _diContainer;
        private SignalBus _signalBus;
        private ITask _processingTask;

        public bool HasAnyTask => _tasks.Count > 0;

        public TaskService(DiContainer diContainer, SignalBus signalBus)
        {
            _diContainer = diContainer;
            _signalBus = signalBus;
        }
        public void Initialize()
        {
            CreateToken();
        }

        public void CreateToken()
        {
            _cancellationToken = new CancellationTokenSource();
        }

        public void AddTask(ITask task, bool toFirst = false)
        {
            _diContainer.Inject(task);
            if (toFirst)
            {
                // DebugLogger.Log($"Adding task <#{task.GetType().Name}({task})#> on first", this, new Color(0.85f, 0.5f, 0.8f));
                _tasks.AddFirst(task);
            }
            else
            {
                // DebugLogger.Log($"Adding task <#{task.GetType().Name}({task})#> on last", this, new Color(0.85f, 0.5f, 0.8f));
                _tasks.AddLast(task);
            }
            if (!_isRunning && !_isPaused)
            {
                ProcessTask();
            }

        }

        public void RemoveUnprocessedTask(ITask task)
        {
            if (_processingTask != task)
            {
                var unprocessedTask = _tasks.Find(task);
                if (unprocessedTask != null)
                {
                    _tasks.Remove(unprocessedTask);
                    // DebugLogger.Log($"Removing unprocessed task <#{task.GetType().Name}({task})#>", this, new Color(0.85f, 0.11f, 0.12f));
                }
            }
        }

        public void PauseTasks()
        {
            _isPaused = true;
            // DebugLogger.Log("Pause processing tasks", this, new Color(1f, 0.67f, 0.3f));

        }

        public void ResumeTasks()
        {
            _isPaused = false;
            if (!_isRunning)
            {
                //DebugLogger.Log("Continue processing tasks", this, new Color(0.05f, 0.75f, 0.1f));
                ProcessTask();
            }
        }

        private void ProcessTask()
        {
            if (_tasks.Count <= 0)
            {
                // DebugLogger.Log("All tasks are processed", this, new Color(0.05f, 0.75f, 0.1f));
                _isRunning = false;
                return;
            }

            var currentTask = _tasks.First;
            _tasks.RemoveFirst();
            // DebugLogger.Log($"Processing Task: <#{currentTask.Value.GetType().Name}({currentTask.Value})#>", this, new Color(0.58f, 0.67f, 0.85f));
            _isRunning = true;

            _processingTask = currentTask.Value;
            currentTask.Value.StartTask(_cancellationToken, OnTaskCompleted);
        }

        async void OnTaskCompleted(float delay)
        {
            if (delay > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }

            // DebugLogger.Log($"Task is complete. <#{_processingTask.GetType().Name}({_processingTask})#>", this, new Color(0.05f, 0.75f, 0.1f));
            _processingTask = null;
            if (!_isPaused)
            {
                ProcessTask();
            }
            else
            {
                _isRunning = false;
                // DebugLogger.Log($"Task is completed and next task is not started because task service is paused. Current task count : {_tasks.Count}", this);
            }
        }

        public void Dispose()
        {
            _tasks.Clear();
            _isRunning = false;

            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
                _cancellationToken.Dispose();
                _cancellationToken = null;
            }
        }


        public void WithSignal<T, U>() where T : ITask where U : struct

        {

            Type taskType = typeof(T);

            var taskInstance = (ITask)Activator.CreateInstance(taskType);

            _signalBus.Fire<U>();

            AddTask(taskInstance);

        }





        public void WithAction<T>(Action<T> startFunction) where T : CustomTask

        {

            Type taskType = typeof(T);



            var taskInstance = (CustomTask)Activator.CreateInstance(taskType, (Action<object>)(o => startFunction((T)o)));



            if (startFunction != null && taskInstance is T)

            {

                AddTask(taskInstance);

            }
        }
    }

    public class CustomTask : GameTask

    {

        private readonly Action<object> _startFunction;



        public CustomTask(Action<object> startFunction)

        {

            _startFunction = startFunction;

        }



        public override void StartTask(CancellationTokenSource cancellationTokenSource, TaskCompleteCallback taskCompleteCallback)

        {

            base.StartTask(cancellationTokenSource, taskCompleteCallback);



            if (_startFunction != null)

            {

                _startFunction.Invoke(this);

            }

        }

    }

}



