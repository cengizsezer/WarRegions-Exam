using MyProject.Core.Controllers;
using MyProject.Core.Services;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

public class LockUIGameTask : GameTask
{
    #region Injection

    
    private InputController _inputController;
    private LeanTouch _leanTouch;
    private ScreenController _screenController;

    [Inject]
    private void Construct(
        
         InputController inputController
        , ScreenController screenController
        , LeanTouch leanTouch)
    {
       
        _inputController = inputController;
        _leanTouch = leanTouch;
        _screenController = screenController;
    }

    #endregion

    public override void StartTask(CancellationTokenSource cancellationTokenSource, TaskCompleteCallback taskCompleteCallback)
    {
        base.StartTask(cancellationTokenSource, taskCompleteCallback);

        _leanTouch.enabled = false;
        
        CompleteTask();
    }
}
