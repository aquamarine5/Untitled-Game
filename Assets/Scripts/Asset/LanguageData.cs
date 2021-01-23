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
[System.Serializable]
public struct BuildmapLanguageData
{
    public string CaveDigging;
    public string StartBuild;
    public string PlantGlass;
}
