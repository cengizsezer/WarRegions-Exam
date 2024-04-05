using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public partial class Damageable : MonoBehaviour
{
    public struct DamageMessage
    {
        public MonoBehaviour damager;
        public int amount;
       
    }

}
public partial class Damageable : MonoBehaviour
{
    public int currentHitPoints { get; private set; }

    public UnityEvent OnDeath, OnReceiveDamage;
    public List<MonoBehaviour> onDamageMessageReceivers;
   
    System.Action schedule;
   
    public void ApplyDamage(DamageMessage data)
    {
        if (currentHitPoints <= 0)
        {
            return;
        }
       
        currentHitPoints -= data.amount;

        if (currentHitPoints <= 0)
        {
            schedule += OnDeath.Invoke;
        }
        else
        {
            OnReceiveDamage.Invoke();
        }

        var messageType = currentHitPoints <= 0 ? MobMessageType.DEAD : MobMessageType.DAMAGED;

        for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
        {
            IMessageReceiver receiver = onDamageMessageReceivers[i] as IMessageReceiver;
            receiver.OnReceiveMessage(messageType, this, data);
        }
    }

    void LateUpdate()
    {
        if (schedule != null)
        {
            schedule();
            schedule = null;
        }
    }

}
