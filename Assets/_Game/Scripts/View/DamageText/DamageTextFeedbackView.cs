using MyProject.Core.Services;
using DG.Tweening;
using MEC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class DamageTextFeedbackView : BaseView, IPoolable<IMemoryPool>
{
    [SerializeField] private TextMeshProUGUI infoText;

    private IMemoryPool _pool;
    private CoroutineHandle _coroutineHandle;

    #region Injection

    private CoroutineService _coroutineService;
    private Camera _mainCamera;
    [Inject]
    private void Construct
    (   
        CoroutineService coroutineService
        ,Camera mainCamera
        
    )
    {
        _mainCamera = mainCamera;
        _coroutineService = coroutineService;
    }

    #endregion

    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
        Initialize();
    }

    private void LateUpdate()
    {
        transform.LookAt(_mainCamera.transform);
    }
    public override void Initialize()
    {
        _coroutineHandle = _coroutineService.StartCoroutine(Despawn());
    }

    private IEnumerator<float> Despawn()
    {
        yield return Timing.WaitForSeconds(2f);
        _pool.Despawn(this);
    }

    public void Init(Transform parent,float f, Color color)
    {
        transform.SetParent(parent);
        transform.localScale = Vector3.one*3f;
        transform.localPosition = new Vector3(0f,1f,0f);

        infoText.text = (f).ToString("0");
        infoText.color = color;

        if (DOTween.IsTweening(this)) DOTween.Complete(this);
        infoText.DOFade(1f, .1f);
        transform.DOShakePosition(.4f);
        transform.DOLocalMoveY(2, .1f)
            .OnComplete(() => infoText.DOFade(0, 1f).SetEase(Ease.InQuad));
    }

    public void OnDespawned()
    {
        if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
    }

    private void OnApplicationQuit()
    {
        if (_coroutineHandle.IsRunning) _coroutineService.StopCoroutine(_coroutineHandle);
    }

    public class Factory : PlaceholderFactory<DamageTextFeedbackView>
    {
    }

    public class Pool : MonoPoolableMemoryPool<IMemoryPool, DamageTextFeedbackView>
    {
    }
}
