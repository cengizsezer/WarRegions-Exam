using MyProject.Core.Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

public class LevelFailGameTask : GameTask
{
    #region Injection

    private PopupService _popupService;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(PopupService popupService
        , SignalBus signalBus)
    {
        _popupService = popupService;
        _signalBus = signalBus;
    }

    #endregion

    public void Init()
    {
       

    }

    public override void StartTask(CancellationTokenSource cancellationTokenSource,
        TaskCompleteCallback taskCompleteCallback)
    {
        base.StartTask(cancellationTokenSource, taskCompleteCallback);
        
        _signalBus.Fire(new LevelFailSignal());
        SetDelay(1f).Forget();
    }

    private async UniTaskVoid SetDelay(float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        LevelFailPopupParameters levelFailPopupParameters = new LevelFailPopupParameters()
        {
           
        };
       
        _popupService.AddPopup(levelFailPopupParameters);

        CompleteTask();
    }
}
