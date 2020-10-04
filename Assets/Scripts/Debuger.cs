
using UnityEngine;
using UnityEngine.UI;

class Debuger : MonoBehaviour
{
    public Text text;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Application.logMessageReceivedThreaded += OnReceiveLogMessage;
    }

    void OnReceiveLogMessage(string condition,string strckTrace,LogType type)
    {

    }
}