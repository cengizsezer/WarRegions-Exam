using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelMountainSettings
{
    public GridStartLayout gridStartLayout = new GridStartLayout(9, 9);
    [SerializeField] protected int[] mountainIndex;
    [SerializeField] protected string hexColor;
    public int[] MountainIndex => mountainIndex;
    public string HexColor => hexColor;
}
