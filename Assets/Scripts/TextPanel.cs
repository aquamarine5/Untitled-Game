using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class TextPanel : MonoBehaviour
{
    public GameObject NormalPanel;
    public GameObject CommandPanel;
    public CatalogueScript catalogueScript;
    public InputField inputField;
    public Text text;
    public ScrollRect scrollRect;
    public void OnRun()
    {
        if (text.text != "")
        {
            try
            {
                print(11);
                XLuaControl.luaEnv.DoString(inputField.text);
            }
            catch(LuaException ex)
            {
                Debuger.OnReceiveLogMessage(ex.Message, ex.StackTrace, LogType.Error,false);
            }
            inputField.text = "";
            inputField.ActivateInputField();
            Canvas.ForceUpdateCanvases();       //关键代码
            scrollRect.verticalNormalizedPosition = 0f;  //关键代码
            Canvas.ForceUpdateCanvases();
        }
    }
    public void GoToNormalPanel()
    {
        CommandPanel.SetActive(false);
        NormalPanel.SetActive(true);
    }
    public void GoToCommandPanel()
    {
        NormalPanel.SetActive(false);
        CommandPanel.SetActive(true);
    }
}
