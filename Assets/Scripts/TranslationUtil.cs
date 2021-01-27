using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TranslationUtil
{
    static BuildmapLanguageData buildmapLanguageData = LanguageLibrary.S.buildmapLanguageData;
    static readonly Dictionary<string, string> d = new Dictionary<string, string>()
    {
        ["CaveDigging"] = buildmapLanguageData.CaveDigging,
        ["GlassBuilding"] = buildmapLanguageData.PlantGlass,
        ["StartBuild"] = buildmapLanguageData.StartBuild
    };
    public static string ConvertToString(this TilemapSpawn.BuildMapStatus buildMapStatus) => d[buildMapStatus.ToString()];
}
