using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public abstract class BaseView : MonoBehaviour, IInitializable, IDisposable
{
    #region Injection

    protected SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }
    #endregion

    public abstract void Initialize();

    public virtual void Dispose()
    {

    }

    public void DestroyView(float delay = 0)
    {
        Dispose();
        Destroy(gameObject, delay);
    }

}

