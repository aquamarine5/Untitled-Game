using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

class Lake : MonoBehaviour
{
    private void Start()
    {
#if UNITY_EDITOR
        Debug.Log(1);
        AssetBundle asb = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"//__data.assetbundle");
        string[] ass = asb.GetAllAssetNames();
        foreach(string o in ass)
        {
            GameObject oo=(GameObject)Instantiate(asb.LoadAsset(o));
            Texture2D t = oo.GetComponentInChildren<Image>().sprite.texture;
            Debug.Log(AssetDatabase.GetAssetPath(t));
            //Debug.LogError(1);
            StartCoroutine(ScreenShot_ReadPixels(t));
            /*
            FileStream fs = new FileStream("D://a.jpg", FileMode.Create, FileAccess.Write);
            fs.Write(t.EncodeToPNG(), 0, t.EncodeToPNG().Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            Debug.LogError(1);*/
        }
#endif
    }
    private IEnumerator ScreenShot_ReadPixels(Texture2D t)
    {

        yield return new WaitForEndOfFrame();

        //读取像素

        //Texture2D tex = new Texture2D(Screen.width, Screen.height);

        t.ReadPixels(new Rect(0, 0, t.width, t.height), 0, 0);

        t.Apply();

        //保存读取的结果

        string path = "D:/ScreenShot_ReadPixels.png";

        System.IO.File.WriteAllBytes(path, t.EncodeToPNG());

    }
}