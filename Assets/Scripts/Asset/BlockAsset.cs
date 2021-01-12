using System;
using UnityEngine.Tilemaps;
using UnityEngine;

[Serializable]
public class BlockAsset : ScriptableObject
{
    public TileBase glass;
    public TileBase glass_dirt;
    public AnimatedTile water;
    public TileBase black;
}