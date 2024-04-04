using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyProject.Core.Const.GlobalConsts;

[CreateAssetMenu(fileName = nameof(LevelData), menuName = AssetMenuName.DATA + nameof(LevelData))]
public class LevelData : ScriptableObject
{
    public GridData gridViewData;
    public MountainData[] mountains;
    public SeaData[] seas;
}

[System.Serializable]
public class GridData
{
    public int xSize, zSize;
    public float xOffset, zOffset;
}

[System.Serializable]
public class MountainData
{
    public int xIndex;
    public int zIndex;
}

[System.Serializable]
public class SeaData
{
    public int xIndex;
    public int zIndex;
}
