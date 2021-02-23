using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapChunk : MonoBehaviour
{
    public Dictionary<(int, int), PolygonCollider2D> colliderIndex = new Dictionary<(int, int), PolygonCollider2D>();

    /// <summary>
    /// Convert world position to chunk local position<br/>
    /// For example, <br/>
    /// Chunk position is (1,1)  (<b>Chunk transform position is (10,10)</b>)<br/>
    /// World position is (18,16) ,so this local position is (8,6)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public (int, int) ConvertToLocalPosition(Vector2Int position) => (position.x - (int)transform.position.x, position.y - (int)transform.position.y);

    public void AddCollider((int,int) position,Vector2[] physicsShape)
    {
        PolygonCollider2D polygonCollider2D;
        if (colliderIndex.ContainsKey(position))
            polygonCollider2D = colliderIndex[position];
        else 
        {
            polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
            colliderIndex.Add(position, polygonCollider2D);
        }
        polygonCollider2D.points = physicsShape;
    }
}
