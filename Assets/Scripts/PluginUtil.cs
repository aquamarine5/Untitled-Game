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
    public static string ConvertToString(this TilemapSpawn.BuildMapStatus buildMapStatus)
    {
        switch (buildMapStatus)
        {
            case TilemapSpawn.BuildMapStatus.CaveDigging:
                return "挖洞中（#）";
            case TilemapSpawn.BuildMapStatus.GlassBuilding:
                return "";
            default:return "";
        }
    }
}