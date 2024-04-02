using System.Threading;

namespace MyProject.Core.Services
{
    public interface ITask
    {
        void StartTask(CancellationTokenSource cancellationTokenSource, TaskCompleteCallback taskCompleteCallback);
        void CompleteTask(float delay = 0);


    }
}

