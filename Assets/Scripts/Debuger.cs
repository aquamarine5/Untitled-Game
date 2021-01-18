using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Debuger : NetworkBehaviour
{
    public static Text s_text;
    public Text text;
    private void Awake()
    {
        s_text = text;
        DontDestroyOnLoad(this);
        Application.logMessageReceivedThreaded += OnReceiveLogMessage;

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
        //s_text.text += "\n* "+color.Replace("&", condition + "\n" + strckTrace);
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
