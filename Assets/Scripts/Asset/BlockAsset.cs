using System;
using UnityEngine.Tilemaps;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName ="BlockAsset",menuName ="AssetDatabase/BlockAsset")]
public class BlockAsset : ScriptableObject
{
    /// <summary>
    /// <seealso cref="CatalogueScript"/>
    /// </summary>
    public static BlockAsset BlockAssetInstance { get; set; }

    [Header("Common Tile")]
    public TileBase glass;
    public TileBase glass_dirt;
    public TileBase black;
    [Header("Animated Tile")]
    public AnimatedTile water;
    
    public PrefabTile torch;
    public enum BlockAssetId
    {
        glass=0,
        glass_dirt=1,
        water=2,
        black=3,
    }
    public enum ItemAssetId
    {
        torch=1001
    }
}
public class BlockLibrary
{
    /// <summary>
    /// <seealso cref="CatalogueScript"/>
    /// </summary>
    public static BlockAsset BlockAssetInstance { get; set; }
}