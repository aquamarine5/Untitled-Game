using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    public Strata.BoardGenerator sbg;
    public CompositeCollider2D cc2d;
    public Text showSeedText;
    public GameObject loadingPanel;
    public Animator anim;
    public BlockAsset blockAsset;
    public Tilemap tilemap;
    public static int x, y = 0;
    public void BuildMap()
    {
        int seed = Random.Range(0, 2333333);
        showSeedText.text = seed.ToString();
        Random.InitState(seed);
        loadingPanel.SetActive(true);
        x = 250; y = 500;
        
        StartCoroutine(StartBuildMap());
        
    }
    IEnumerator StartBuildMap()
    {
        anim.SetBool("isRuning", true);
        yield return StartCoroutine(sbg.BuildLevel());
        anim.SetBool("isRuning", false);
        loadingPanel.SetActive(false);
        showSeedText.text += "\nShape:"+cc2d.shapeCount;
    }
}
