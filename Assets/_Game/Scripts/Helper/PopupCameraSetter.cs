using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Canvas))]
public class PopupCameraSetter : MonoBehaviour
{
    [SerializeField] Canvas _popUpCanvas;
    [Inject]
    private void Construct([Inject(Id = "uiCamera")] Camera camera)
    {
        _popUpCanvas.worldCamera = camera;
    }
}
