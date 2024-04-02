using System;
using System.Threading;

namespace MyProject.Core.Services
{
    public class GameTask : ITask
    {

        protected TaskCompleteCallback _taskCompleteCallback;
        private event Func<float, bool> TaskCompleted = delegate { return false; };
        private bool isCompleted = false;

        public virtual void StartTask(CancellationTokenSource cancellationTokenSource, TaskCompleteCallback taskCompleteCallback)

        {

            _taskCompleteCallback = taskCompleteCallback;

            TaskCompleted = (delay) => isCompleted;

        }



        public virtual void CompleteTask(float delay = 0)

        {

            _taskCompleteCallback?.Invoke(delay);

            isCompleted = true;

            TaskCompleted?.Invoke(delay);

        }

        public bool IsCompleted()

        {

            return isCompleted;

        }

    }

}
