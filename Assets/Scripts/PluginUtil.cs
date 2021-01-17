using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapPlugin
{
    public static TileBase[,] tm = new TileBase[TilemapSpawn._targetSize.x, TilemapSpawn._targetSize.y];
    public static Dictionary<(int, int), TileBase> tmDictionary = new Dictionary<(int, int), TileBase>();
    public static void Fill(this Tilemap map, TileBase tile, Vector2Int start, Vector2Int end)
    {
        int xDir = start.x < end.x ? 1 : -1;
        int yDir = start.y < end.y ? 1 : -1;
        int xCols = 1 + Mathf.Abs(start.x - end.x);
        int yCols = 1 + Mathf.Abs(start.y - end.y);
        for (int x = 0; x < xCols; x++)
        {
            for (int y = 0; y < yCols; y++)
            {
                map.ReSetTile((x * xDir + start.x, y * yDir + start.y), tile);
            }
        }
    }
    public static void ReSetTile(this Tilemap tilemap, (int, int) vector2Int, TileBase tile) => SetTilemap(tilemap, vector2Int, tile);
    public static void ReSetTile(this Tilemap tilemap, Vector2Int vector2Int, TileBase tile) => SetTilemap(tilemap, (vector2Int.x, vector2Int.y), tile);
    [System.Obsolete] public static void ReSetTile(this Tilemap tilemap, Vector3Int vector3Int, TileBase tile) => SetTilemap(tilemap, (vector3Int.x, vector3Int.y), tile);

    public static void ReSetTiles(this Tilemap tilemap, (int, int)[] vector2Ints, TileBase tile) => SetTilemap(tilemap, vector2Ints, tile);
    public static void ReSetTiles(this Tilemap tilemap, (int, int)[] vector2Ints, TileBase[] tiles) => SetTilemap(tilemap, vector2Ints, tiles);

    static void SetTilemap(Tilemap tilemap, (int, int) vector2Int, TileBase tile)
    {
        tilemap.SetTile(new Vector3Int(vector2Int.Item1, vector2Int.Item2, 0), tile);
        if (tmDictionary.ContainsKey(vector2Int)) tmDictionary[vector2Int] = tile;
        else tmDictionary.Add(vector2Int, tile);
    }
    static void SetTilemap(Tilemap tilemap, (int, int)[] vector3Ints, TileBase tile)
    {
        List<Vector3Int> resultVector3s = new List<Vector3Int>();
        List<TileBase> tileBases = new List<TileBase>();
        foreach (var i in vector3Ints)
        {
            resultVector3s.Add(new Vector3Int(i.Item1, i.Item2, 0));
            tileBases.Add(tile);
            tm[i.Item1, i.Item2] = tile;
        }
        tilemap.SetTiles(resultVector3s.ToArray(), tileBases.ToArray());
    }
    static void SetTilemap(Tilemap tilemap, (int, int)[] vector3Ints, TileBase[] tiles)
    {
        List<Vector3Int> resultVector3s = new List<Vector3Int>();
        foreach (var i in vector3Ints)
        {
            resultVector3s.Add(new Vector3Int(i.Item1, i.Item2, 0));
        }
        tilemap.SetTiles(resultVector3s.ToArray(), tiles);
        for (int i = 0; i < vector3Ints.Length; i++)
        {
            tm[vector3Ints[i].Item1, vector3Ints[i].Item2] = tiles[i];
        }
    }
}
public static class OthersPlugin
{
    static BuildmapLanguageData buildmapLanguageData = CatalogueScript.ReturnThis().languageData.buildmapLanguageData;
    static readonly Dictionary<string, string> d = new Dictionary<string, string>()
    {
        ["CaveDigging"] = buildmapLanguageData.CaveDigging,
        ["GlassBuilding"] = buildmapLanguageData.PlantGlass,
        ["StartBuild"] = buildmapLanguageData.StartBuild
    };
    public static string ConvertToWebBase(this ulong bytes)
    {
        if (1024 > bytes) return bytes.ToString("F2") + "B";
        else if ((bytes == Mathf.Pow(1024, 1)) || (Mathf.Pow(1024, 2) > bytes)) return (bytes / 1024).ToString("F2") + "KB";
        else if ((bytes == Mathf.Pow(1024, 2)) || (Mathf.Pow(1024, 3) > bytes)) return (bytes / Mathf.Pow(1024, 2)).ToString("F2") + "MB";
        else if ((bytes == Mathf.Pow(1024, 3)) || (Mathf.Pow(1024, 4) > bytes)) return (bytes / Mathf.Pow(1024, 3)).ToString("F2") + "GB";
        else return "Failed";
    }
    public static string ConvertToString(this TilemapSpawn.BuildMapStatus buildMapStatus) => d[buildMapStatus.ToString()];
}
