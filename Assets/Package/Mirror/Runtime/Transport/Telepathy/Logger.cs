// A simple logger class that uses Console.WriteLine by default.
// Can also do Logger.LogMethod = Debug.Log for Unity etc.
// (this way we don't have to depend on UnityEngine.DLL and don't need a
//  different version for every UnityEngine version here)
using System;

namespace Telepathy
{
    public static class Logger
    {
        public static bool isUseUnityDebug = false;
        public static string message;
        public static int logType;
        public static bool isUpdateMessageInNextFrame = false;

        //public static Action<string> Log = Console.WriteLine;
        //public static Action<string> LogWarning = Console.WriteLine;
        //public static Action<string> LogError = Console.Error.WriteLine;
        
        public static void Log(string message)
        {
            Logger.message = message;
            isUpdateMessageInNextFrame = true;
            logType = 3;
            if (isUseUnityDebug) UnityEngine.Debug.Log(message);
            else Console.WriteLine(message);
        }
        public static void LogWarning(string message)
        {
            Logger.message = message;
            isUpdateMessageInNextFrame = true;
            logType = 2;
            if (isUseUnityDebug) UnityEngine.Debug.LogWarning(message);
            else Console.WriteLine(message);
        }
        public static void LogError(string message)
        {
            Logger.message = message;
            isUpdateMessageInNextFrame = true;
            logType = 0;
            if (isUseUnityDebug) UnityEngine.Debug.LogError(message);
            else Console.Error.WriteLine(message);
        }
    }
}
