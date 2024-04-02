using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingProgressBarView : BaseView
{
    [SerializeField] private RectTransform _fillRect;

    public override void Initialize()
    {
        _signalBus.Subscribe<LoadingProgressBarSignal>(OnLoadingProgressBarSignal);

        _fillRect.sizeDelta = new Vector2(0f, 58f);

        gameObject.SetActive(true);
    }

    private void OnLoadingProgressBarSignal(LoadingProgressBarSignal signal)
    {
        _fillRect.DOKill();

        var newSize = new Vector2(signal.ProgressValue / 100f * 1108, 58f);

        _fillRect.DOSizeDelta(newSize, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (signal.ProgressValue >= 100)
            {
                DestroyView();
            }
        });
    }

    public override void Dispose()
    {
        _fillRect.DOKill();

        _signalBus.TryUnsubscribe<LoadingProgressBarSignal>(OnLoadingProgressBarSignal);
    }
}
