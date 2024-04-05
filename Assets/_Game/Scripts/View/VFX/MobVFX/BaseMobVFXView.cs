using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMobVFXView : BaseView
{
    public GameObject Trail;
    public override void Initialize()
    {
       
    }

    public abstract void SetView(Sprite sprite);

    public abstract void VoidDespawn();
}
