using MyProject.Core.Const;
using MyProject.Core.Data;
using MyProject.Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPopupParameters : BasePopupParameters
{
    public UserLevelData PlayerLevelData;
    public int Level;

    public override bool IsBundleRequired()
    {
        return false;
    }

    public override string PopupName()
    {
        return GlobalConsts.PopupName.LevelUpPopup;
    }

    public override float CloseDuration()
    {
        return 0f;
    }
}
