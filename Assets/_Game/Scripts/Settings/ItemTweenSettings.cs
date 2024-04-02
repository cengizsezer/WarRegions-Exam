using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;


namespace MyProject.Core.Settings
{
    [CreateAssetMenu(fileName = nameof(ItemTweenSettings), menuName = AssetMenuName.SETTINGS + nameof(ItemTweenSettings))]
    public class ItemTweenSettings : ScriptableObject
    {
        #region Movement

        [FoldoutGroup("Movement")] public float DragSpeed;
        [FoldoutGroup("Movement")] public float DragDeadZone = 1;
        [FoldoutGroup("Movement")] public float LandDuration;
        [FoldoutGroup("Movement")] public float MoveDuration;
        [FoldoutGroup("Movement")] public float RushDuration;

        [FoldoutGroup("Movement")] public Ease MoveEase;
        [FoldoutGroup("Movement")] public Ease OrderMoveEase;
        [FoldoutGroup("Movement")] public Ease OrderScaleEase;


        #endregion


    }

}

