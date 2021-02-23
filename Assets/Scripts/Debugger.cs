using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Telepathy.Logger;
public class Debugger : MonoBehaviour
{
    public static Text s_text;
    public Text text;

    static Dictionary<ColourfulTextCollection, string> translationDictionary = new Dictionary<ColourfulTextCollection, string>()
    {
        [ColourfulTextCollection.Black] = "black"
    };
    private void Awake()
    {
        s_text = text;
        DontDestroyOnLoad(this);
        Application.logMessageReceivedThreaded += OnReceiveLogMessage;
    }
    private void Update()
    {
        if (isUpdateMessageInNextFrame)
        {
            OnReceiveLogMessage(message, "",(LogType)logType, false);
            if (logType == 0) Debug.LogError(message);
            else if (logType == 3) Debug.Log(message);
            else if (logType == 2) Debug.LogWarning(message);
            message = null;
            isUpdateMessageInNextFrame = false;
        }
    }
    public static void OnReceiveLogMessage(string condition,string strckTrace,LogType type)
    {
        string color = "";
        switch (type)
        {
            case LogType.Warning: color = "<color=yellow>&</color>";break;
            case LogType.Log:color = "&";break;
            case LogType.Error:color = "<color=red>&</color>";break;
            case LogType.Exception:color = "<color=red>&</color>";break;
        }
        s_text.text += $"\n* {color.Replace("&", condition + "\n" + strckTrace)}";
    }
    public static void OnReceiveLogMessage(string condition, string strckTrace, LogType type, bool showStackTrace) =>
        OnReceiveLogMessage(condition, showStackTrace ? strckTrace : "", type);
    
    public static void ColourfulText(string str,ColourfulTextCollection colourfulTextCollection) { }
}
/// <summary>
/// Using for <see cref="Debugger.ColourfulText(string, ColourfulTextCollection)"/>
/// <br/>See also <seealso cref="Color"/>
/// </summary>
public enum ColourfulTextCollection
{
    /// <summary>
    /// RGBA is (0,0,0,1)
    /// </summary>
    Black,
    /// <summary>
    /// RGBA is (0,0,1,1)
    /// </summary>
    Blue,
    /// <summary>
    /// RGBA is (1,1,1,1)
    /// </summary>
    White,
    /// <summary>
    /// RGBA is (0,1,1,1)
    /// </summary>
    Cyan,
    /// <summary>
    /// RGBA is (0.5,0.5,0.5,1)
    /// </summary>
    Gray,
    /// <summary>
    /// RGBA is (0,1,0,1)
    /// </summary>
    Green,
    /// <summary>
    /// RGBA is (1,0,1,1)
    /// </summary>
    Magenta,
    /// <summary>
    /// RGBA is (1,0,0,1)
    /// </summary>
    Red,
    /// <summary>
    /// RGBA is (1,0.92,0.016,1)
    /// </summary>
    Yellow
}