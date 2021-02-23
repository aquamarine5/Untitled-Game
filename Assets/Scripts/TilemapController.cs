using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TilemapUtil;

public class TilemapController : MonoBehaviour
{
    public GameObject tilemapChunkMaster;
    public GameObject tilemapChunkPrefab;
    public TilemapChunk AddChunk((int,int) position)
    {
        return Instantiate(tilemapChunkPrefab,new Vector3(position.Item1 * 10, position.Item2 * 10),
            Quaternion.identity, tilemapChunkMaster.transform).GetComponent<TilemapChunk>();
    }
    
    /// <summary>
    /// Convert tilemap position to chunk position<br/>
    /// For example,<br/>
    /// if position in range(0-9)<b>(inclube 0 and 9)</b> will return 0, also (10-19) will return 1 .
    /// <br/>See also <seealso cref="TilemapChunk"/>.
    /// </summary>
    /// <param name="position">Tilemap position</param>
    /// <returns>Chunk position</returns>
    public static (int, int) ConvertToChunkPosition(Vector2Int position) => (position.x / 10, position.y / 10);

    public static (int, int) ConvertToChunkPosition((int, int) position) => (position.Item1 / 10, position.Item2 / 10);

}
