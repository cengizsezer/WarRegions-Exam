using MyProject.Core.Enums;
using MyProject.Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject.Core.Services
{
    public abstract class BasePopupParameters
    {
        public abstract string PopupName();

        public PopupPriority PopupPriority = PopupPriority.NORMAL;
        public ITask Task;

        public virtual bool IsBundleRequired()
        {
            return false;
        }

        public virtual float CloseDuration()
        {
            return 0.5f;
        }

        public virtual bool ShouldTurnOffCamera { get; set; } = false;
        public virtual bool ShouldDisableUI { get; set; } = false;

        public bool OverridePlaneDistance = false;
        public float PlaneDistance;
        public Transform PopupParent;
    }
}

