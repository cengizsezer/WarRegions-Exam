using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ParticleHitCurrencyEvents : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    #region Injection

    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    #endregion
    private void OnParticleCollision(GameObject other)
    {
        _signalBus.Fire(new ParticleHitSignal(other));
    }
}
