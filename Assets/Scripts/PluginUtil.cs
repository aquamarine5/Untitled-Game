using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapPlugin
{
    public static TileBase[,] tm = new TileBase[TilemapSpawn._targetSize.x, TilemapSpawn._targetSize.y];
    public static Dictionary<Vector2Int, TileBase> tmDictionary = new Dictionary<Vector2Int, TileBase>();
    public static void Fill(this Tilemap map, TileBase tile, Vector3Int start, Vector3Int end)
    {
        int xDir = start.x < end.x ? 1 : -1;
        int yDir = start.y < end.y ? 1 : -1;
        int xCols = 1 + Mathf.Abs(start.x - end.x);
        int yCols = 1 + Mathf.Abs(start.y - end.y);
        for (int x = 0; x < xCols; x++)
        {
            for (int y = 0; y < yCols; y++)
            {
                Vector3Int tilePos = start + new Vector3Int(x * xDir, y * yDir, 0);
                map.ReSetTile((Vector2Int)tilePos,tile);
            }
        }
    }
    public static void ReSetTile(this Tilemap tilemap, Vector2Int vector2Int, TileBase tile) => SetTilemap(tilemap, vector2Int, tile);
    [System.Obsolete]
    public static void ReSetTile(this Tilemap tilemap, Vector3Int vector3Int, TileBase tile) => SetTilemap(tilemap, (Vector2Int)vector3Int, tile);
    [System.Obsolete]
    public static void ReSetTiles(this Tilemap tilemap, Vector3Int[] vector3Ints, TileBase[] tiles) => SetTilemap(tilemap, vector3Ints, tiles);

    static void SetTilemap(Tilemap tilemap,Vector2Int vector2Int, TileBase tile)
    {
        tilemap.SetTile((Vector3Int)vector2Int, tile);
        if (tmDictionary.ContainsKey(vector2Int)) tmDictionary[vector2Int] = tile;
        else tmDictionary.Add(vector2Int, tile);
    }
    static void SetTilemap(Tilemap tilemap,Vector3Int[] vector3Ints,TileBase[] tiles)
    {
        tilemap.SetTiles(vector3Ints, tiles);
        for (int i = 0; i < vector3Ints.Length; i++)
        {
            tm[vector3Ints[i].x, vector3Ints[i].y] = tiles[i];
        }
    }
}
public static class OthersPlugin {
    static Dictionary<string, string> d = new Dictionary<string, string>();
    static bool isDictionaryDefine = false;
    public static string ConvertToWebBase(this ulong bytes)
    {
        if (1024 > bytes) return bytes.ToString("F2") + "B";
        else if ((bytes == Mathf.Pow(1024, 1)) || (Mathf.Pow(1024, 2) > bytes)) return (bytes / 1024).ToString("F2") + "KB";
        else if ((bytes == Mathf.Pow(1024, 2)) || (Mathf.Pow(1024, 3) > bytes)) return (bytes / Mathf.Pow(1024, 2)).ToString("F2") + "MB";
        else if ((bytes == Mathf.Pow(1024, 3)) || (Mathf.Pow(1024, 4) > bytes)) return (bytes / Mathf.Pow(1024, 3)).ToString("F2") + "GB";
        else return "Failed";
    }
    public static string ConvertToString(this TilemapSpawn.BuildMapStatus buildMapStatus)
    {
        BuildmapLanguageData buildmapLanguageData = CatalogueScript.ReturnThis().languageData.buildmapLanguageData;
        if (!isDictionaryDefine)
        {
            d.Add(TilemapSpawn.BuildMapStatus.CaveDigging.ToString(), buildmapLanguageData.CaveDigging);
            d.Add(TilemapSpawn.BuildMapStatus.GlassBuilding.ToString(), buildmapLanguageData.PlantGlass);
            d.Add(TilemapSpawn.BuildMapStatus.StartBuild.ToString(), buildmapLanguageData.StartBuild);
            isDictionaryDefine = true;
        }
        return d[buildMapStatus.ToString()];
    }
}
public static class RandomSeedPlugin
{
    static int nowSeed;
    public static int NowSeed { get => nowSeed;set 
        {
            nowSeed = value;
            Random.InitState(nowSeed);
        } }
    public static int SetSeed(this int seed)
    {
        Debug.Log(seed);
        Random.InitState(seed);
        nowSeed = seed;
        return seed;
    }
}