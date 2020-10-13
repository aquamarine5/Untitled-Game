using UnityEngine.Tilemaps;
using UnityEngine;

public static class TilemapPlugin
{
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
                map.SetTile(tilePos, tile);
            }
        }
    }
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
        CatalogueScript catalogueScript = CatalogueScript.ReturnThis();
        switch (buildMapStatus)
        {
            case TilemapSpawn.BuildMapStatus.StartBuild:
                return catalogueScript.languageData.Buildmap_StartBuild;
            case TilemapSpawn.BuildMapStatus.CaveDigging:
                return catalogueScript.languageData.Buildmap_CaveDigging;
            case TilemapSpawn.BuildMapStatus.GlassBuilding:
                return catalogueScript.languageData.Buildmap_PlantGlass;
            default:return "";
        }
    }
}