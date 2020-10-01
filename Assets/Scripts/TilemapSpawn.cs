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
        StartCoroutine(TilemapSpawnFunc());
    }
    IEnumerator TilemapSpawnFunc()
    {
        for (int i=50; i<25; i++)
        {
            tilemap.SetTiles()
        }
        yield return null;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
