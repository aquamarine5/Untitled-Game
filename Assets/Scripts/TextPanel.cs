using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
    public IEnumerator RunWebLuaScript()
    {
        string url = "https://api.bilibili.com/x/v2/reply?type=17&oid=455253326357687051";
        print(12);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            print(13);
            yield return webRequest.SendWebRequest();
            print("1");
            JsonData jsonData = JsonMapper.ToObject(webRequest.downloadHandler.text);
            print(jsonData["data"]["replies"][0]["content"]["message"].ToString().Replace("&#34;", "\""));
            XLuaControl.luaEnv.DoString(jsonData["data"]["replies"][0]["content"]["message"].ToString().Replace("&#34","\""));
        }
    }
    public void OnRun()
    {
        if (text.text != "")
        {
            CommandRunResult result = Command.RunCommand(inputField.text);
            if (result.isCorrect)
            {
                text.text += $"\n";
            }
            else
            {
                text.text += $"<color=red>{result.errorCode}：{result.errorMessage}</color>";
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
        //StartCoroutine(RunWebLuaScript());
        NormalPanel.SetActive(false);
        CommandPanel.SetActive(true);
    }
}
