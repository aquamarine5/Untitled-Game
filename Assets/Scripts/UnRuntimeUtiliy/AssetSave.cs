using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;

class AssetSave
{
    public static class ScriptableObjectUtility
    {
        [MenuItem("CreateAsset/Asset")]
        public static void CreateAssetMenu()
        {
            CreateAsset<WeaponAsset>();
        }
        [MenuItem("CreateAsset/Load")]
        public static void Load()
        {

        }
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Not select files, select files first! ");
                return;
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New" + typeof(T).ToString() + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
[Serializable]
public class WeaponAsset : ScriptableObject
{
    public WeaponBase wbScript = null;
    public Object weaponScript=null;
    public Sprite[] weaponSprite;
    public float rotate;
    public Sprite[] attackSprite;
}