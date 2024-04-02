using System;

namespace MyProject.Core.Enums
{
    [Serializable]
    public enum TimeFormat
    {
        Timer,
        TimerLessThanHour,
        Duration
    }

    public enum BuildType
    {
        DevelopmentBuild,
        ProdBuild
    }

    public enum CurrencyType
    {
        Coin = 0,
       Mana=1
    }

    public enum LogTypes
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }

    public enum ScreenState
    {
        Loading,
        MainMenu,
        Map
    }

    public enum VFXType
    {
        TakeDamage,
        Dead,
        Spawn
    }

    public enum GridState
    {
      Filled=0,
      Free=1
    }

    public enum GameplayMobType
    {
        None = 0,
        Player = 1,
        Enemy=2
       
    }

    public enum CharMobType
    {
        Char_0 = 0,
        Char_1 = 1,
        Char_2 = 2,
        Char_3 = 3,
        Char_4 = 4
    }
   
    public struct Tweens
    {
        public static readonly string SPAWN1 = "Spawn_1";
        public static readonly string SPAWN2 = "Spawn_2";
        public static readonly string MERGE = "Merge";
        public static readonly string DESPAWN = "Despawn";
    }

    public enum CurrencyUpdateType
    {
        Gain = 0,
        Consume = 1
    }

    public enum PopupPriority
    {
        NORMAL,
        IMMEDIATE
    }

    public enum FlagState
    {
        Unavailable,
        Available,
        WaitingToBeAvailable
    }

    public enum ItemName
    {
        Coin = 0,
        Mana=1,
        Player=2,
        Enemy=3
    }

    public enum BoardState
    {
        None,
        Idle,
        Active
    }

    public enum AnimStates
    {
        Idle,
        Run,
        Attack
    }

    public enum LoginProgressBarValue
    {
        Start = 0,
        FacebookInitalized = 25,
        RemoteConfigsInitalized = 50,
        LoadProducts = 75,
        GameReady = 100
    }
}


