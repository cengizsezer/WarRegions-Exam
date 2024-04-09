using MyProject.Core.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

[CreateAssetMenu(fileName = nameof(LevelData), menuName = AssetMenuName.DATA + nameof(LevelData))]
public class LevelData : ScriptableObject
{
    [SerializeField] private LevelGroundTypeSettings levelGroundTypeSettings;
    [SerializeField] private LevelMilitaryBaseTypeSettings militaryBaseTypeSettings;
    [SerializeField] private LevelMountainSettings levelMountainSettings;
    public LevelGroundTypeSettings LevelGroundTypeSettings => levelGroundTypeSettings;
    public LevelMilitaryBaseTypeSettings MilitaryBaseTypeSettings => militaryBaseTypeSettings;
    public LevelMountainSettings LevelMountainSettings => levelMountainSettings;
}

