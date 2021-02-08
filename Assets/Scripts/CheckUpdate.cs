using LitJson;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static PanelControl;

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
    public Text version;
    static string downloadPath;
    bool isBackstage = false;
    string newUrl;
    ulong downloads = 0;
    bool isOnStart = true;
    float downloadsCD = 0;
    private void Awake()
    {
        downloadPath = Application.persistentDataPath + "/Application/newApplicationUrl.apk";
    }
    public static string ConvertToWebByte(ulong bytes)
    {
        if (1024 > bytes) return bytes.ToString("F2") + "B";
        else if ((bytes == Mathf.Pow(1024, 1)) || (Mathf.Pow(1024, 2) > bytes)) return (bytes / 1024).ToString("F2") + "KB";
        else if ((bytes == Mathf.Pow(1024, 2)) || (Mathf.Pow(1024, 3) > bytes)) return (bytes / Mathf.Pow(1024, 2)).ToString("F2") + "MB";
        else if ((bytes == Mathf.Pow(1024, 3)) || (Mathf.Pow(1024, 4) > bytes)) return (bytes / Mathf.Pow(1024, 3)).ToString("F2") + "GB";
        else return "Failed";
    }
    public void CheckUpdated()
    {
        PanelInstanic.ClosePanel(PanelInstanic.panelCollection.UpdatePanel);
        if (isBackstage == false)
        {
            StartCoroutine(WebRequests($"https://api.bilibili.com/x/space/acc/info?mid={b_id}&jsonp=jsonp"));
        }
    }
    private void Start()
    {
        version.text = Application.version;
        StartCoroutine(WebRequests($"https://api.bilibili.com/x/space/acc/info?mid={b_id}&jsonp=jsonp",false));
    }
    IEnumerator WebRequests(string url,bool isUpdate=true)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                text.text = webRequest.error + "\n" + webRequest.downloadHandler.text;
                text.color = Color.red;
            }
            else
            {
                JsonData jsonData = JsonMapper.ToObject(webRequest.downloadHandler.text);
                string newVersion= ((string)jsonData["data"]["sign"]).Split('*')[0];
                if (newVersion!= Application.version)
                {
                    newUrl = "http://cloud.ananas.chaoxing.com/view/fileviewDownload?objectId=" + ((string)jsonData["data"]["sign"]).Split('*')[1];
                    if (isUpdate) { yield return StartCoroutine(DownloadApplicationFile(downloadPath)); }
                    else
                    {
                        version.text = $"<color=red>{Application.version}(NEW:{newVersion})</color>";
                    }
                }
                else
                {
                    text.color = Color.green;
                    text.text = "Your application\nis NEW!";
                    version.text = $"<color=green>{Application.version}</color>";
                };
            }
        }
    }
    public void OnBackstage()
    {
        isBackstage = true;
        PanelInstanic.ChangePanel(PanelInstanic.panelCollection.UpdatePanel);
    }
    public IEnumerator DownloadApplicationFile(string downloadFileName, bool isCommand = false)
    {
        using (UnityWebRequest downloader = UnityWebRequest.Get(newUrl))
        {
            downloader.SetRequestHeader(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36 Edg/85.0.564.51");
            downloader.downloadHandler = new DownloadHandlerFile(downloadFileName);
            downloader.SendWebRequest();
            ulong size = downloader.downloadedBytes;
            while (!downloader.isDone)
            {
                downloadsCD += Time.deltaTime;
                if (isOnStart && downloadsCD >= 0.1) speed.text = ConvertToWebByte(downloader.downloadedBytes * 10 - 0) + "/s";
                if (isOnStart && downloadsCD >= 0.5) speed.text = ConvertToWebByte(downloader.downloadedBytes * 2 - 0) + "/s"; isOnStart = false;
                if (downloadsCD >= 1)
                {
                    speed.text = ConvertToWebByte(downloader.downloadedBytes - downloads) + "/s";
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

        AndroidJavaObject jo = new AndroidJavaObject("com.syz.unitygamePlugin.Main");
        jo.Call("installApk", apkPath);
#endif
    }

}
