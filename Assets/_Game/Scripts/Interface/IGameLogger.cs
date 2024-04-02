using System;

public delegate void LogFileCallback(string logFilePath);

public interface IGameLogger
{
    void I(string log, object[] arguments = null, Exception e = null);
    void W(string log, object[] arguments = null, Exception e = null);
    void E(string log, object[] arguments = null, Exception e = null);
    void EnableNativeLogs();
}
