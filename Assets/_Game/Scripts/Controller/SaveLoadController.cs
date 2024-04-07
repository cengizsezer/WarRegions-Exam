using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoadController
{
    const string KEY_LEVEL = "levels";
    public static void IncreaseLevel() => PlayerPrefs.SetInt(KEY_LEVEL, GetLevel() + 1);
    public static int GetLevel() => PlayerPrefs.GetInt(KEY_LEVEL, 0);
}
