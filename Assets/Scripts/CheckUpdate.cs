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
    public struct ApplicationUrls
    {
        public static string a0_1_3;
        public static string a0_2_4 = "15257aa10f8c6ce6e81dd05ad746729a";
        public static string a0_2_10 = "";
    }
    public enum ApplicationUrlEnum
    {
        a0_1_3=1,
        a0_2_4=2,
        a0_2_10=3
    }
    public void CheckUpdated()
    {
        StartCoroutine(WebRequests("url"));
    }
    public IEnumerator WebRequests(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                JsonData jsonData = JsonMapper.ToObject(webRequest.downloadHandler.text);
                if ((string)jsonData["data"]["sign"] != ApplicationUrls.a0_2_10)
                {
                    yield return StartCoroutine(DownloadVideoFile01("", Application.persistentDataPath+"/"+"a0_2_10.apk", slider));
                };
            }
        }
        print(1);
    }
    public IEnumerator DownloadVideoFile01(string uri, string downloadFileName, Slider sliderProgress)
    {
        using (UnityWebRequest downloader = UnityWebRequest.Get(uri))
        {
            downloader.downloadHandler = new DownloadHandlerFile(downloadFileName);

            print("开始下载");
            downloader.SendWebRequest();
            print("同步进度条");
            while (!downloader.isDone)
            {
                //print(downloader.downloadProgress);
                sliderProgress.value = downloader.downloadProgress;
                sliderProgress.GetComponentInChildren<Text>().text = (downloader.downloadProgress * 100).ToString("F2") + "%";
                yield return null;
            }

            if (downloader.error != null)
            {
                Debug.LogError(downloader.error);
            }
            else
            {
                print("下载结束");
                sliderProgress.value = 1f;
                sliderProgress.GetComponentInChildren<Text>().text = 100.ToString("F2") + "%";
            }
        }
    }
}
