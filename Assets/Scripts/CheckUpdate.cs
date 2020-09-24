using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CheckUpdate : MonoBehaviour
{
    public Slider slider;
    public GameObject panel;
    public Text text;
    bool isDone=false;
    string newUrl;
    public struct ApplicationUrls
    {
        public static string a0_1_3;
        public static string a0_2_4 =  "15257aa10f8c6ce6e81dd05ad746729a";
        public static string a0_2_10 = "443a1d5136d0f6e9ae0fae327d19c355";
    }
    public enum ApplicationUrlEnum
    {
        a0_1_3=1,
        a0_2_4=2,
        a0_2_10=3
    }
    public void CheckUpdated()
    {
        panel.SetActive(true);
        StartCoroutine(WebRequests("https://api.bilibili.com/x/space/acc/info?mid=474085001&jsonp=jsonp"));
        
    }
    public IEnumerator WebRequests(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                text.text=webRequest.error + "\n" + webRequest.downloadHandler.text;
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                JsonData jsonData = JsonMapper.ToObject(webRequest.downloadHandler.text);
                if ((string)jsonData["data"]["sign"] != ApplicationUrls.a0_2_10)
                {
                    newUrl = "http://d0.ananas.chaoxing.com/download/"+ (string)jsonData["data"]["sign"];
                    yield return StartCoroutine(DownloadApplicationFile(Application.persistentDataPath+"/"+"a0_2_10.apk", slider));
                }
                else {
                    text.text = "已经是最新版本";
                };
            }
        }
        print(1);
    }
    public IEnumerator DownloadApplicationFile(string downloadFileName, Slider sliderProgress)
    {
        using (UnityWebRequest downloader = UnityWebRequest.Get(newUrl))
        {
            downloader.SetRequestHeader(
                "User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36 Edg/85.0.564.51");
            downloader.downloadHandler = new DownloadHandlerFile(downloadFileName);

            print("开始下载");
            downloader.SendWebRequest();
            print("同步进度条");
            while (!downloader.isDone)
            {
                //print(downloader.downloadProgress);
                sliderProgress.value = downloader.downloadProgress;
                text.text = (downloader.downloadProgress * 100).ToString("F2") + "%";
                yield return null;
            }

            if (downloader.error != null)
            {
                text.text=downloader.error;
            }
            else
            {
                isDone = downloader.isDone;
                print("下载结束");
                sliderProgress.value = 1f;
                text.text = 100.ToString("F2") + "%";
                Application.OpenURL("file://"+ Application.persistentDataPath + "/" + "a0_2_10.apk");
            }
        }
    }
}
