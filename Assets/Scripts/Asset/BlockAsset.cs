using System;
using UnityEngine.Tilemaps;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName ="BlockAsset",menuName ="AssetDatabase/BlockAsset")]
public class BlockAsset : ScriptableObject
{
    [Header("Common Tile")]
    public TileBase glass;
    public TileBase glass_dirt;
    public TileBase black;
    [Header("Animated Tile")]
    public AnimatedTile water;
    
    public PrefabTile torch;
    public enum BlockAssetId
    {
        /// <summary>
        /// 土块
        /// </summary>
        glass=0,
        /// <summary>
        /// 草
        /// </summary>
        glass_dirt=1,
        /// <summary>
        /// 水
        /// </summary>
        water=2,
        /// <summary>
        /// BlackDefaultNoCollider.tile
        /// </summary>
        black=3,
    }
    public enum ItemAssetId
    {
        /// <summary>
        /// 火把
        /// </summary>
        torch=1001
    }
}