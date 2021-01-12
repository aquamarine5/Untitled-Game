using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CheckUpdate : MonoBehaviour
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
    bool isBackstage = false;
    string newUrl;
    ulong downloads = 0;
    bool isOnStart = true;
    float downloadsCD = 0;
    public struct ApplicationUrls:IEnumerable
    {
        public string path;
        public string a0_1_3;
        public string a0_2_4;
        public string a0_2_10;
        public string a0_3_4;
        public string a0_3_4_r1;
        public string a0_3_5;
        public string a0_3_7;
        public string a0_3_8;
        public string a0_3_11;
        public string a0_4_1;
        public string a0_4_2;
        public string a0_5_5;
        public string a0_5_8;
        public string a0_5_9;
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)path).GetEnumerator();
        }
    }
    ApplicationUrls applicationUrls = new ApplicationUrls() {
        a0_1_3 = "",
        a0_2_4 = "15257aa10f8c6ce6e81dd05ad746729a",
        a0_2_10 = "443a1d5136d0f6e9ae0fae327d19c355",
        a0_3_4 = "66e9b08d91c336a0854389027a943298",
        a0_3_4_r1 = "a032db337f5c5e9e336388c09621286e",
        a0_3_5 = "92594e55d1493cc18c35b760ee6a1eba",
        a0_3_7 = "01693f6e4065f8bca0ca15a612458049",
        a0_3_8 = "64b149fda30c802726f9bb5d300443f1",
        a0_3_11 = "71c1ee39b8a9febd848635ddeafe5592",
        a0_5_8 = "00cdd954dec2ca2be66df94e28b06cdf",
        a0_5_9 = "4beac5a8b37f38f5ed654d0110684610",
    };
    private void Awake()
    {
        applicationUrls.path = Application.persistentDataPath + "/" + "newApplicationUrl.apk";
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
                if (((string)jsonData["data"]["sign"]).Split('*')[0] != Application.version+"1")
                {
                    newUrl = "http://d0.ananas.chaoxing.com/download/"+ ((string)jsonData["data"]["sign"]).Split('*')[1];
                    yield return StartCoroutine(DownloadApplicationFile(applicationUrls.path, slider));
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
    IEnumerator DownloadApplicationFile(string downloadFileName, Slider sliderProgress)
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
                sliderProgress.value = downloader.downloadProgress;
                text.text = (downloader.downloadProgress * 100).ToString("F2") + "%";
                yield return null;
            }
            if (downloader.error != null)
            {
                text.color = Color.red;
                text.text=downloader.error;
            }
            else
            {
                sliderProgress.value = 1f;
                text.text = 100.ToString("F2") + "%";
                InstallApp(applicationUrls.path);
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