using MyProject.Core.Controllers;
using MyProject.Core.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelController : BaseController
{
   

    LevelData crntLevelData;

    public LevelData GetLevelData() => crntLevelData;

    #region Injection

    private LevelSettings _levelSettings;
    
    [Inject]
    protected virtual void Construct
    (
        LevelSettings levelSettings
    )
    {
       _levelSettings = levelSettings;

    }
    #endregion

    public void SetLevelData()
    {
        if (_levelSettings.LevelDataList.Count == 0) return;

        int levelID = _levelSettings.LevelDataList.Count >= 1 ? SaveLoadController.GetLevel() % _levelSettings.LevelDataList.Count:0;

        crntLevelData = _levelSettings.LevelDataList[levelID];
        
    }

    protected override void OnInitialize()
    {
        SetLevelData();
    }

    protected override void OnApplicationReadyToStart()
    {
        
    }

    protected override void OnDispose()
    {
       
    }
}
