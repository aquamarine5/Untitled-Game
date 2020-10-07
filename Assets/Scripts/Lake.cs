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
        AssetBundle asb = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"//__data");
        string[] ass = asb.GetAllAssetNames();
        print(ass.Length);
        foreach(var s in ass)
        {
            print(s);
            var a=(LayerMask)LayerMask.GetMask("");
        }
        foreach(string o in ass)
        {
            GameObject oo=(GameObject)Instantiate(asb.LoadAsset(o));
            
            Texture2D t = oo.GetComponentInChildren<Image>().sprite.texture;
            /*
            AssetImporter ai = AssetImporter.GetAtPath(o);
            print(ai.assetPath);
            print(ai.userData);
            Debug.Log(o);
            Debug.Log(AssetDatabase.GetAssetPath(oo));
            StartCoroutine(ScreenShot_ReadPixels(t));*/
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