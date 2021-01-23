using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
    public static Text s_text;
    public Text text;
    private void Awake()
    {
        s_text = text;
        DontDestroyOnLoad(this);
        Application.logMessageReceivedThreaded += OnReceiveLogMessage;
    }
    private void Update()
    {
        if (Telepathy.Logger.isUpdateMessageInNextFrame)
        {
            OnReceiveLogMessage(Telepathy.Logger.message,"",(LogType)Telepathy.Logger.logType,false);
            if (Telepathy.Logger.logType == 0) Debug.LogError(Telepathy.Logger.message);
            else if (Telepathy.Logger.logType == 3) Debug.Log(Telepathy.Logger.message);
            else if (Telepathy.Logger.logType == 2) Debug.LogWarning(Telepathy.Logger.message);
            Telepathy.Logger.message = null;
            Telepathy.Logger.isUpdateMessageInNextFrame = false;
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
        s_text.text += "\n* "+color.Replace("&", condition + "\n" + strckTrace);
    } 
    public static void OnReceiveLogMessage(string condition, string strckTrace, LogType type, bool showStackTrance)
    {
        string color = "";
        switch (type)
        {
            case LogType.Warning: color = "<color=yellow>&</color>"; break;
            case LogType.Log: color = "&"; break;
            case LogType.Error: color = "<color=red>&</color>"; break;
            case LogType.Exception: color = "<color=red>&</color>"; break;
        }
        if (showStackTrance)
        {
            s_text.text += "\n* " + color.Replace("&", condition + "\n" + strckTrace);
        }
        else s_text.text += "\n* " + color.Replace("&", condition);
    }
}
