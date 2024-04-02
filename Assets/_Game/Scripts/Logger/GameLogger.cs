using System;
using UnityEngine;
using Zenject;
using MyProject.Core.Enums;

public class GameLogger : IInitializable, IDisposable, IGameLogger
{
    public void Initialize()
    {

    }

    public void Dispose()
    {

    }

    private void Log(string message, object[] arguments = null, LogTypes type = LogTypes.INFO, Exception e = null)
    {
        switch (type)
        {
            case LogTypes.DEBUG:
            case LogTypes.INFO:
                DebugLogger.Log(message, this, Color.cyan);
                break;
            case LogTypes.WARNING:
                DebugLogger.LogWarning(message, this, Color.cyan);
                break;
            case LogTypes.ERROR:
                DebugLogger.LogError(message, this, Color.cyan);
                break;
        }

        //Crashlytics.Log(message);
    }

    public void I(string log, object[] arguments, Exception e)
    {
        Log(log, arguments, LogTypes.INFO, e);
    }

    public void W(string log, object[] arguments, Exception e)
    {
        Log(log, arguments, LogTypes.WARNING, e);
    }

    public void E(string log, object[] arguments, Exception e)
    {
        Log(log, arguments, LogTypes.ERROR, e);
    }

    public void EnableNativeLogs()
    {
        if (!Application.isEditor)
        {
            Application.logMessageReceived += HandleNativeLogMessages;
        }
    }

    private void HandleNativeLogMessages(string message, string stackTrace, UnityEngine.LogType type)
    {
        if (type == UnityEngine.LogType.Log)
        {
            return;
        }

        if (!string.IsNullOrEmpty(stackTrace))
        {
            message += "\n" + stackTrace;
        }

        switch (type)
        {
            case UnityEngine.LogType.Exception:
            case UnityEngine.LogType.Error:
                Log(message, null, LogTypes.ERROR);
                break;
        }
    }

}

