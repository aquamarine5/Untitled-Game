using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSpawn : MonoBehaviour
{
    public BlockAsset blockAsset;
    public Tilemap tilemap;
    private void Awake()
    {
        print(1);
        tilemap.BoxFill(new Vector3Int(1,1,1), blockAsset.glass_dirt, 1, 1, 15, 15);
        tilemap.SetTile(new Vector3Int(0, 0, 0), blockAsset.glass);
        //StartCoroutine(TilemapSpawnFunc());
    }
    IEnumerator TilemapSpawnFunc()
    {
        
        //tilemap.BoxFill(new Vector3Int(),blockAsset.glass)
        for (int i=0; i<25; i++)
        {
            
            //tilemap.SetTiles();
        }
        yield return null;
    }
    Vector3Int CreateVector(int x, int y) { return new Vector3Int(x, y, 0); }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
