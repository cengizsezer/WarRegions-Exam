using MyProject.Core.CustomAttribute;
using MyProject.Core.Data;
using MyProject.Core.Enums;
using Lean.Touch;
using UnityEngine;

[Signal]
public readonly struct ApplicationReadyToStartSignal
{
}

[Signal]
public struct ScreenStateChangedSignal
{
    public ScreenState CurrentScreenState;
}

[Signal]
public struct FlagStatusChangedSignal
{
    public string FlagName;
}

[Signal]
public readonly struct LoadingProgressBarSignal
{
    public readonly int ProgressValue;

    public LoadingProgressBarSignal(int progressValue)
    {
        ProgressValue = progressValue;
    }
}

[Signal]
public struct ButtonDeactivateSignal
{
}

[Signal]
public struct ParticleHitSignal
{
    public readonly GameObject ParticleSystemForceField;

    public ParticleHitSignal(GameObject particleSystemForceField)
    {
        ParticleSystemForceField = particleSystemForceField;
    }
}

[Signal]
public struct ButtonActivateSignal
{
}


[Signal]
public readonly struct CoinUpdatedSignal
{
    public readonly Vector2 StartPosition;
    public readonly bool Animate;
    public readonly bool Reset;

    public CoinUpdatedSignal(Vector2 startPosition, bool animate = false, bool reset = false)
    {
        StartPosition = startPosition;
        Animate = animate;
        Reset = reset;
    }
}

[Signal]
public readonly struct ManaUpdatedSignal
{
    public readonly Vector2 StartPosition;
    public readonly bool Animate;

    public ManaUpdatedSignal(Vector2 startPosition, bool animate = false
      )
    {
        StartPosition = startPosition;
        Animate = animate;
    }
}

[Signal]
public readonly struct SettingsPopupClosedSignal
{
}

[Signal]
public readonly struct SettingsPopupOpenedSignal
{
}


#region InputSignals

[Signal]
public readonly struct FingerDownSignal
{
    public readonly LeanFinger Finger;

    public FingerDownSignal(LeanFinger finger)
    {
        Finger = finger;
    }
}

[Signal]
public readonly struct FingerUpdateSignal
{
    public readonly LeanFinger Finger;

    public FingerUpdateSignal(LeanFinger finger)
    {
        Finger = finger;
    }
}

[Signal]
public readonly struct FingerUpSignal
{
    public readonly LeanFinger Finger;

    public FingerUpSignal(LeanFinger finger)
    {
        Finger = finger;
    }
}

[Signal]
public readonly struct FingerPinchSignal
{
    public readonly float PinchValue;

    public FingerPinchSignal(float pinchValue)
    {
        PinchValue = pinchValue;
    }
}

#endregion




[Signal]
public readonly struct BoardReadySignal
{
}

[Signal]
public readonly struct LevelSuccessSignal
{
    public readonly int Count;

    public LevelSuccessSignal(int count)
    {
        Count = count;
    }
}

[Signal]
public readonly struct WaveComplateSignal
{

}

[Signal]
public readonly struct LevelFailSignal
{
    public readonly int Count;

    public LevelFailSignal(int count)
    {
        Count = count;
    }
}

[Signal]
public readonly struct XPUpdatedSignal
{
    public readonly int Count;

    public XPUpdatedSignal(int count)
    {
        Count = count;
    }
}

[Signal]
public readonly struct EnergyTimerUpdatedSignal
{
}

[Signal]
public readonly struct StartTransitionSignal
{
    public readonly float Duration;

    public StartTransitionSignal(float duration)
    {
        Duration = duration;
    }
}

[Signal]
public readonly struct ItemTakenFromBoardRewardAreaSignal
{
}
[Signal]
public readonly struct PlayButtonClickSignal
{
}
[Signal]
public readonly struct AttackButtonClickSignal
{
    
}
[Signal]
public readonly struct ContinueButtonClickSignal
{

}
[Signal]
public readonly struct StartEnemySpawnTimerSignal
{
}
[Signal]
public readonly struct StopEnemySpawnTimerSignal
{
}

[Signal]
public readonly struct BoardInfoSignal
{
    public readonly string Info;
    public readonly Vector3 Position;

    public BoardInfoSignal(string info, Vector3 position)
    {
        Info = info;
        Position = position;
    }
}
[Signal]
public readonly struct CurrencyGainSignal
{
    public readonly ItemName ItemName;
    public readonly int Count;
    public readonly Vector3 Position;

    public CurrencyGainSignal(ItemName itemName, int count, Vector3 position)
    {
        ItemName = itemName;
        Count = count;
        Position = position;
    }
}

[Signal]
public readonly struct CharacterItemSelectedSignal
{
    public readonly ItemGroupData ItemGroupData;
    public readonly Vector2Int Coordinates;
    public readonly int Level;

    public CharacterItemSelectedSignal(ItemGroupData itemGroupData
        , Vector2Int coordinates, int level)
    {
        ItemGroupData = itemGroupData;
        Coordinates = coordinates;
        Level = level;
    }
}

[Signal]
public struct MoveCountViewChangedSignal
{
   
}





