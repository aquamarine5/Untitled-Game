using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CheckUpdate : MonoBehaviour
{
    public string nowVersion;
    public Slider slider;
    public GameObject panel;
    public Text text;
    public Text speed;
    public bool isBackstage = false;
    //bool isDone=false;
    string newUrl;
    float downloads = 0;
    float downloadsCD = 0;
    public struct ApplicationUrls
    {
        public static string path = Application.persistentDataPath + "/" + "a0_3_5.apk";
        public static string a0_1_3;
        public static string a0_2_4 =  "15257aa10f8c6ce6e81dd05ad746729a";
        public static string a0_2_10 = "443a1d5136d0f6e9ae0fae327d19c355";
        public static string a0_3_4 =  "66e9b08d91c336a0854389027a943298";
        public static string a0_3_4_r1="a032db337f5c5e9e336388c09621286e";
        public static string a0_3_5 =  "92594e55d1493cc18c35b760ee6a1eba";
        public static string a0_3_7 =  "01693f6e4065f8bca0ca15a612458049";
        public static string a0_3_8 =  "64b149fda30c802726f9bb5d300443f1";
        public static string a0_3_10 = "71c1ee39b8a9febd848635ddeafe5592";
    }
    public void CheckUpdated()
    {
        panel.SetActive(true);
        if (isBackstage == false)
        {
            print(isBackstage);
            StartCoroutine(WebRequests("https://api.bilibili.com/x/space/acc/info?mid=474085001&jsonp=jsonp"));
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
                if (((string)jsonData["data"]["sign"]).Split('*')[0] != nowVersion)
                {
                    newUrl = "http://d0.ananas.chaoxing.com/download/"+ ((string)jsonData["data"]["sign"]).Split('*')[1];
                    yield return StartCoroutine(DownloadApplicationFile(ApplicationUrls.path, slider));
                }
                else {
                    text.color = Color.green;
                    text.text = "已经是最新版本";
                };
            }
        }
        print(1);
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
            
            print("开始下载");
            downloader.SendWebRequest();
            ulong size= downloader.downloadedBytes;
            print(size);
            print("同步进度条");
            while (!downloader.isDone)
            {
                downloadsCD += Time.deltaTime;
                if (downloadsCD >= 1)
                {
                    speed.text = ConvertToData(downloader.downloadedBytes - downloads, "/s");
                    downloads = downloader.downloadedBytes;
                    downloadsCD = 0;
                }
                //print(downloader.downloadedBytes);
                sliderProgress.value = downloader.downloadProgress;
                //text.text=downloader.downloadedBytes.ToString();
                text.text = (downloader.downloadProgress * 100).ToString("F2") + "%";
                yield return null;
            }

            if (downloader.error != null)
            {
                text.text=downloader.error;
            }
            else
            {
                //isDone = downloader.isDone;
                print("下载结束");
                sliderProgress.value = 1f;
                text.text = 100.ToString("F2") + "%";
                InstallApp(ApplicationUrls.path);
            }
        }
    }
    string ConvertToData(float bytes,string inp)
    {
        if (1024 > bytes) return bytes.ToString("F2") + "B" + inp;
        else if ((bytes == Mathf.Pow(1024, 1)) || (Mathf.Pow(1024, 2) > bytes)) return (bytes / 1024).ToString("F2") + "KB" + inp;
        else if ((bytes == Mathf.Pow(1024, 2)) || (Mathf.Pow(1024, 3) > bytes)) return (bytes / Mathf.Pow(1024, 2)).ToString("F2") + "MB" + inp;
        else if ((bytes == Mathf.Pow(1024, 3)) || (Mathf.Pow(1024, 4) > bytes)) return (bytes / Mathf.Pow(1024, 3)).ToString("F2") + "GB" + inp;
        else return "Failed";
    }
    void InstallApp(string apkPath)
    {
#if UNITY_ANDROID
        
        AndroidJavaObject jo = new AndroidJavaObject("com.syz.unitygame.AndroidPlugin");
        jo.Call("installApk", apkPath);
#endif
    }

}
