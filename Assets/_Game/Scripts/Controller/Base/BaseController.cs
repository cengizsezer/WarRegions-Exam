using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace MyProject.Core.Controllers
{
    public abstract class BaseController : IInitializable, IDisposable
    {
        protected IGameLogger _gameLogger;
        protected SignalBus _signalBus;

        [Inject]
        private void Construct(IGameLogger gameLogger, SignalBus signalBus)
        {
            _gameLogger = gameLogger;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            OnInitialize();
        }

        protected abstract void OnInitialize();
        protected abstract void OnApplicationReadyToStart();

        public void Dispose()
        {

            OnDispose();
        }

        protected abstract void OnDispose();
    }
}


