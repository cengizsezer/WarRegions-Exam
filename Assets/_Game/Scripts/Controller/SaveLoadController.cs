using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoadController
{
    const string KEY_LEVEL = "levels";
    public static void IncreaseLevel() => PlayerPrefs.SetInt(KEY_LEVEL, GetLevel() + 1);
    public static int GetLevel() => PlayerPrefs.GetInt(KEY_LEVEL, 0);



    const string KEY_COIN = "coincoinqwe";
    public static int GetCoin() => PlayerPrefs.GetInt(KEY_COIN, 0);
    public static void AddCoin(int add)
    {
        int current =  add;
        PlayerPrefs.SetInt(KEY_COIN, current);
    }
}
