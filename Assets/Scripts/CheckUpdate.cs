using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.UI;
using Mirror;

public class CheckUpdate : NetworkBehaviour
{
    [Tooltip("B站UID（用于小型数据库）")]
    public int b_id;
    [Tooltip("显示下载进度的进度条")]
    public Slider slider;
    [Tooltip("显示更新窗口的GameObject")]
    public GameObject panel;
    [Tooltip("显示下载进度的Text")]
    public Text text;
    [Tooltip("显示下载速度的Text")]
    public Text speed;
    static string downloadPath;
    bool isBackstage = false;
    string newUrl;
    ulong downloads = 0;
    bool isOnStart = true;
    float downloadsCD = 0;
    private void Awake()
    {
        downloadPath = Application.persistentDataPath + "/" + "newApplicationUrl.apk";
    }
    public void CheckUpdated()
    {
        panel.SetActive(true);
        if (isBackstage == false)
        {
            StartCoroutine(WebRequests("https://api.bilibili.com/x/space/acc/info?mid="+b_id+"&jsonp=jsonp"));
        }
    }
    IEnumerator WebRequests(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                text.text=webRequest.error + "\n" + webRequest.downloadHandler.text;
                text.color = Color.red;
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                JsonData jsonData = JsonMapper.ToObject(webRequest.downloadHandler.text);
                if (((string)jsonData["data"]["sign"]).Split('*')[0] != Application.version)
                {
                    newUrl = "http://d0.ananas.chaoxing.com/download/"+ ((string)jsonData["data"]["sign"]).Split('*')[1];
                    yield return StartCoroutine(DownloadApplicationFile(downloadPath));
                }
                else {
                    text.color = Color.green;
                    text.text = "Your application\nis NEW!";
                };
            }
        }
    }
    public void OnBackstage()
    {
        isBackstage = true;
        print(isBackstage);
        panel.SetActive(false);
    }
    public IEnumerator DownloadApplicationFile(string downloadFileName,bool isCommand=false)
    {
        using (UnityWebRequest downloader = UnityWebRequest.Get(newUrl))
        {
            downloader.SetRequestHeader(
                "User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36 Edg/85.0.564.51");
            downloader.downloadHandler = new DownloadHandlerFile(downloadFileName);
            downloader.SendWebRequest();
            ulong size= downloader.downloadedBytes;
            while (!downloader.isDone)
            {
                
                downloadsCD += Time.deltaTime;
                if (isOnStart && downloadsCD >= 0.1) speed.text = (downloader.downloadedBytes * 10 - 0).ConvertToWebBase() + "/s";
                if (isOnStart && downloadsCD >= 0.5) speed.text = (downloader.downloadedBytes * 2 - 0).ConvertToWebBase() + "/s"; isOnStart = false;
                if (downloadsCD >= 1)
                {
                    speed.text = (downloader.downloadedBytes - downloads).ConvertToWebBase() + "/s";
                    downloads = downloader.downloadedBytes;
                    downloadsCD = 0;
                }
                slider.value = downloader.downloadProgress;
                text.text = (downloader.downloadProgress * 100).ToString("F2") + "%";
                yield return null;
            }
            if (downloader.error != null)
            {
                if (isCommand)
                {

                }
                else
                {
                    text.color = Color.red;
                    text.text = downloader.error;
                }
            }
            else
            {
                slider.value = 1f;
                text.text = 100.ToString("F2") + "%";
                InstallApp(downloadPath);
            }
        }
    }
    void InstallApp(string apkPath)
    {
#if UNITY_ANDROID
        
        AndroidJavaObject jo = new AndroidJavaObject("com.syz.unitygame.AndroidPlugin");
        jo.Call("installApk", apkPath);
#endif
    }
}