using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TilemapUtil;

public class TilemapChunk : MonoBehaviour
{
    /// <summary>
    /// Convert world position to chunk local position<br/>
    /// For example, <br/>
    /// Chunk position is (1,1)  (<b>Chunk transform position is (10,10)</b>)<br/>
    /// World position is (18,16) ,so this local position is (8,6)
    /// </summary>
    /// <param name="position">world position</param>
    /// <returns>chunk local position</returns>
    /// <exception cref="PositionNotInChunkException"/>
    public (int, int) ConvertToLocalPosition(Vector2Int position)
    {
        if ((position.x - transform.position.x) < 0f || (position.y - transform.position.y) < 0f)
            throw new PositionNotInChunkException { requestPosition = position, chunkPosition = ConvertToChunkPosition(this) };
        return (position.x - (int)transform.position.x, position.y - (int)transform.position.y);
    }

    /// <exception cref="PositionNotInChunkException"/>
    public (int, int) ConvertToLocalPosition((int, int) position)
    {
        /*
        if ((position.Item1 - transform.position.x) < 0f || (position.Item2 - transform.position.y) < 0f)
            throw new PositionNotInChunkException { requestPosition = position.ConvertToVector(), chunkPosition = ConvertToChunkPosition(this) };*/
        return (position.Item1 - (int)transform.position.x, position.Item2 - (int)transform.position.y);
    }

    /// <summary>
    /// Convert chunk local position to world position<br/>
    /// For example, <br/>
    /// Chunk position is (1,1)  (<b>Chunk transform position is (10,10)</b>)<br/>
    /// Chunk local position is (8,6) ,so this world position is (18,16), 
    /// </summary>
    /// <param name="position">chunk local position</param>
    /// <returns>world position</returns>
    public (int,int) ConvertToWorldPosition((int, int) position) => (position.Item1 + (int)transform.position.x, position.Item2 + (int)transform.position.y);

    public (int, int) CenterPosition => (5 + (int)transform.position.x, 5 + (int)transform.position.y);

    /// <summary>
    /// TKey is chunk local position (see also <seealso cref="ConvertToLocalPosition((int, int))"/>)<br/>
    /// TValue is <see cref="PolygonCollider2D"/>
    /// </summary>
    public Dictionary<(int, int), PolygonCollider2D> colliderIndex = new Dictionary<(int, int), PolygonCollider2D>();

    public PolygonCollider2D AddCollider((int,int) position,Vector2[] physicsShape = null)
    {
        PolygonCollider2D polygonCollider2D;
        if (colliderIndex.ContainsKey(position))
            polygonCollider2D = colliderIndex[position];
        else 
        {
            polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
            colliderIndex.Add(position, polygonCollider2D);
            polygonCollider2D.offset = new Vector2(0.5f + position.Item1, 0.5f + position.Item2);
            polygonCollider2D.usedByComposite = true;
            polygonCollider2D.SetPath(0, physicsShape);
        }
        return polygonCollider2D;
    }
    /// <summary>
    /// See the collider.
    /// </summary>
    /// <param name="position">chunk local position</param>
    /// <param name="physicsShape">physics shape path</param>
    public void SetCollider((int,int) position,Vector2[] physicsShape)
    {
        if (colliderIndex.ContainsKey(position)) 
        {
            PolygonCollider2D collider = colliderIndex[position];
            collider.SetPath(collider.pathCount + 1, physicsShape);
        }
        else
            AddCollider(position, physicsShape);
    }
}
