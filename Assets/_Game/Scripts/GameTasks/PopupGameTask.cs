using MyProject.Core.Enums;
using MyProject.Core.Services;
using System.Threading;
using Zenject;

public class PopupGameTask : GameTask
{
    private PopupService _popupService;
    private BasePopupParameters _parameters;

    [Inject]
    private void Construct(PopupService popupService)
    {
        _popupService = popupService;
    }

    public void Initialize(BasePopupParameters basePopupParameters)
    {
        _parameters = basePopupParameters;
    }

    public override void StartTask(CancellationTokenSource cancellationTokenSource, TaskCompleteCallback taskCompleteCallback)
    {
        base.StartTask(cancellationTokenSource, taskCompleteCallback);
        if (_parameters.PopupPriority == PopupPriority.IMMEDIATE)
        {
            _popupService.ShowImmediatePopup(_parameters);
        }
        else if (_parameters.PopupPriority == PopupPriority.NORMAL)
        {
            _popupService.ShowPopup(_parameters);
        }
    }

    public override string ToString()
    {
        return _parameters.GetType().Name;
    }
}
