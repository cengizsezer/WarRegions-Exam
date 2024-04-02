using Zenject;
using UnityEngine;


public class BaseScreen : BaseView
{
    public override void Initialize()
    {

    }

    public class Factory : PlaceholderFactory<Object, BaseScreen> { }

}

