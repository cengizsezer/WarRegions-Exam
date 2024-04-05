using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MobMessageType
{
    DAMAGED,
    DEAD,
}

public interface IMessageReceiver
{
    void OnReceiveMessage(MobMessageType type, object sender, object msg);
}
