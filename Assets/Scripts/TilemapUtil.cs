using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;
using static CatalogueScript;
public static class TilemapUtil
{
    #region value
    public static Tilemap mainTilemap;
    public static TileBase[,] tm = new TileBase[TilemapSpawn._targetSize.x, TilemapSpawn._targetSize.y];
    public static Dictionary<(int, int), TilemapChunk> tilemapChunkDictionary = new Dictionary<(int, int), TilemapChunk>();
    public static Dictionary<(int, int), TileBase> tmDictionary = new Dictionary<(int, int), TileBase>();
    public static Dictionary<(int, int), TileBase> itemDictionary = new Dictionary<(int, int), TileBase>();
    #endregion

    #region Tilemap's plugin
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
    #endregion

    public static (int, int) ConvertToTuple(this Vector2Int vector2Int) => (vector2Int.x, vector2Int.y);

    public static Vector2Int ConvertToVector(this (int, int) tuple) => new Vector2Int(tuple.Item1, tuple.Item2);

    /// <summary>
    /// Convert tilemap position to chunk position<br/>
    /// For example,<br/>
    /// if position in range(0-9)<b>(inclube 0 and 9)</b> will return 0, also (10-19) will return 1 .
    /// <br/>See also <seealso cref="TilemapChunk"/>.
    /// </summary>
    /// <param name="position">Tilemap position</param>
    /// <returns>Chunk position</returns>
    public static (int, int) ConvertToChunkPosition(Vector2Int position) => (position.x / 10, position.y / 10);

    public static (int, int) ConvertToChunkPosition((int,int) position) => (position.Item1 / 10, position.Item2 / 10);

    /// <summary>
    /// Convert chunk's transfrom's position chunk position
    /// </summary>
    /// <param name="tilemapChunk">The chunk.</param>
    /// <returns>Chunk position</returns>
    public static (int, int) ConvertToChunkPosition(TilemapChunk tilemapChunk) => ((int)tilemapChunk.transform.position.x / 10, (int)tilemapChunk.transform.position.y / 10);

    public static TilemapChunk AddChunk((int, int) position)
    {
        GameObject gameObject = Object.Instantiate(CatalogueInstance.tilemapChunkPrefab, new Vector3(position.Item1 * 10, position.Item2 * 10),
            Quaternion.identity, CatalogueInstance.tilemapChunkMaster.transform);
        gameObject.name = $"Chunk ({position.Item1},{position.Item2})";
        tilemapChunkDictionary.Add(position, gameObject.GetComponent<TilemapChunk>());
        return gameObject.GetComponent<TilemapChunk>();
    }

    #region RxSetTile
    public static void DefaultSetTile(this Tilemap tilemap, (int, int) vector2Int, TileBase tileBase, bool isCallClient = false)
    {
        tilemap.SetTile(new Vector3Int(vector2Int.Item1, vector2Int.Item2, 0), tileBase);
        if (isCallClient) RpcSetTilemapOnline(tilemap, vector2Int, tileBase);
    }
    
    public static void ReSetTile(this Tilemap tilemap, (int, int) vector2Int, TileBase tile, bool isCallClient = false) =>
        SetTilemap(tilemap, vector2Int, tile, isCallClient);
    public static void ReSetTile(this Tilemap tilemap, Vector2Int vector2Int, TileBase tile, bool isCallClient = false) => 
        SetTilemap(tilemap, (vector2Int.x, vector2Int.y), tile, isCallClient);
    [System.Obsolete] public static void ReSetTile(this Tilemap tilemap, Vector3Int vector3Int, TileBase tile, bool isCallClient = false) => 
        SetTilemap(tilemap, (vector3Int.x, vector3Int.y), tile, isCallClient);


    public static void ReSetTiles(this Tilemap tilemap, (int, int)[] vector2Ints, TileBase tile, bool isCallClient = false) => 
        SetTilemap(tilemap, vector2Ints, tile, isCallClient);
    public static void ReSetTiles(this Tilemap tilemap, (int, int)[] vector2Ints, TileBase[] tiles, bool isCallClient = false) => 
        SetTilemap(tilemap, vector2Ints, tiles, isCallClient);

    static void SetTilemap(Tilemap tilemap, (int, int) vector2Int, TileBase tile, bool isCallClient)
    {
        var chunkPosition = ConvertToChunkPosition(vector2Int);
        // Get chunk, create a new chunk if isn't exist.
        TilemapChunk tilemapChunk = tilemapChunkDictionary.ContainsKey(chunkPosition) 
            ? tilemapChunkDictionary[chunkPosition] 
            : AddChunk(chunkPosition);
        Debug.Log(tile);
        var tilemapPosition = new Vector3Int(vector2Int.Item1, vector2Int.Item2, 0);
        tilemap.SetTile(tilemapPosition, tile);
        var colliderType = tilemap.GetColliderType(tilemapPosition);
        // don't set collider when tile's colliderType is None
        if (colliderType == Tile.ColliderType.Sprite || colliderType == Tile.ColliderType.Grid) 
        {
            // Use tilemap.GetSprite instead TileBase.GetTileData
            Sprite sprite = tilemap.GetSprite(tilemapPosition);
            // Create a buffer list to save data
            List<Vector2> physicsShape = new List<Vector2>();
            // see also : https://docs.unity3d.com/cn/2020.1/ScriptReference/Sprite.GetPhysicsShape.html
            // 精灵可能有孔和不连续部分，因此其轮廓不一定由单个物理形状定义。
            for (int shapeId = 0; shapeId < sprite.GetPhysicsShapeCount(); ++shapeId)
            {
                physicsShape.Clear();
                // GetPhysicsShape will write shape data to physicsShape
                sprite.GetPhysicsShape(shapeId, physicsShape);
                tilemapChunk.SetCollider(tilemapChunk.ConvertToLocalPosition(vector2Int), physicsShape.ToArray());
            }
        }
        if (tmDictionary.ContainsKey(vector2Int)) tmDictionary[vector2Int] = tile;
        else tmDictionary.Add(vector2Int, tile);
        if (isCallClient) RpcSetTilemapOnline(tilemap, vector2Int, tile);
    }
    static void SetTilemap(Tilemap tilemap, (int, int)[] vector3Ints, TileBase tile, bool isCallClient)
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
        if (isCallClient) RpcSetTilemapOnline(tilemap, resultVector3s.ToArray(), tileBases.ToArray());
    }

    static void SetTilemap(Tilemap tilemap, (int, int)[] vector3Ints, TileBase[] tiles, bool isCallClient)
    {
        List<Vector3Int> resultVector3s = new List<Vector3Int>();
        foreach (var i in vector3Ints)
        {
            resultVector3s.Add(new Vector3Int(i.Item1, i.Item2, 0));
        }
        tilemap.SetTiles(resultVector3s.ToArray(), tiles);

        if (isCallClient) RpcSetTilemapOnline(tilemap, resultVector3s.ToArray(), tiles);
        for (int i = 0; i < vector3Ints.Length; i++)
        {
            tm[vector3Ints[i].Item1, vector3Ints[i].Item2] = tiles[i];
        }
    }

    [ClientRpc]
    public static void RpcSetTilemapOnline(this Tilemap tilemap,(int,int) position,TileBase tile)
    {
        tilemap.SetTile(new Vector3Int(position.Item1, position.Item2, 0), tile);
    }

    [ClientRpc]
    public static void RpcSetTilemapOnline(this Tilemap tilemap, Vector3Int[] positions, TileBase[] tiles)
    {
        tilemap.SetTiles(positions, tiles);
    }
    #endregion
}
public class PositionNotInChunkException : System.Exception
{
    public Vector2Int requestPosition;
    public (int, int) chunkPosition;
    public override string ToString() =>
        $"Chunk position : {ConvertTupleToString(chunkPosition)}\nRequest position : {ConvertVector2ToString(requestPosition)}\n{base.ToString()}";
    public override string Message => 
        $"Chunk position : {ConvertTupleToString(chunkPosition)}\nRequest position : {ConvertVector2ToString(requestPosition)}\n{base.Message}";
    string ConvertTupleToString((int, int) input) => $"({input.Item1},{input.Item2})";
    string ConvertVector2ToString(Vector2Int input) => $"({input.x},{input.y})";
}