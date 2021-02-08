using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LanguageData", menuName = "AssetDatabase/LanguageData")]
public class LanguageData:ScriptableObject
{
    public BuildmapLanguageData buildmapLanguageData;
    public string CheckUpdate;
    public string RenderShapeCount;
    public string FPSShow;
    public string SpawnMap;
}
public static class LanguageLibrary
{
    /// <summary>
    /// <seealso cref="CatalogueScript"/>
    /// </summary>
    public static LanguageData LanguageDataInstance { get; set; }
}
[System.Serializable]
public struct BuildmapLanguageData
{
    public string CaveDigging;
    public string StartBuild;
    public string PlantGlass;
}
