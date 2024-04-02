using MyProject.Core.Const;
using MyProject.Core.Services;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ShopPopupParametres: BasePopupParameters
{
    public bool ShouldNavigateToGemArea;
   
    public override string PopupName()
    {
        return GlobalConsts.PopupName.ShopPopup;
    }

    public override float CloseDuration()
    {
        return 0f;
    }
}
public class ShopPopup : BasePopup<ShopPopupParametres>
{
    private Tween _tween;
    private SignalBus _signalBus;
    [Inject]
    private void Construct(SignalBus signalBus)
    {

        _signalBus = signalBus;

    }
    public override void Show()
    {
        base.Show();
    }

    private void ClosePopupFromSignal()//----> button.onclick
    {
        ClosePopup();
    }
    public override void ClosePopup()
    {
        _tween?.Kill();
        _signalBus.Fire<SettingsPopupClosedSignal>();
        base.ClosePopup();
    }
}
