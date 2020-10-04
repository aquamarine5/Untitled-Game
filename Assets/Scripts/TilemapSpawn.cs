using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
}

public class TilemapSpawn : MonoBehaviour
{
    public BlockAsset blockAsset;
    public Tilemap tilemap;
    private void Awake()
    {
        Random.InitState((int)Random.Range(0, 233333333333));
        StartCoroutine(TilemapSpawnFunc());
    }
    IEnumerator TilemapSpawnFunc()
    {

        tilemap.Fill(blockAsset.glass, new Vector3Int(-25, -25, 0), new Vector3Int(25, 0, 0));
        for (int i=0; i<25; i++)
        {
            
            //tilemap.SetTiles();
        }
        yield return null;
    }
    
    //Vector3Int CreateVector(int x, int y) { return new Vector3Int(x, y, 0); }
    void Start()
    {
        Random.InitState(11);
        print(Random.Range(1, 10));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
