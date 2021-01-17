namespace UnityEngine.Tilemaps
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Prefab Tile", menuName = "Tiles/Prefab Tile")]
    public class PrefabTile : TileBase
    {
        public Sprite sprite;
        public Tile.ColliderType m_TileColliderType;
        public GameObject prefab;
        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.colliderType = m_TileColliderType;
            tileData.sprite = sprite;
            tileData.gameObject = prefab;
            base.GetTileData(location, tileMap,ref tileData);
        }
        public void Instantiate(Transform parent) => Instantiate(prefab, parent);
        public void Instantiate(Vector3 position, Quaternion rotate) => Instantiate(prefab, position, rotate);
        public void Instantiate(Vector3 position, Quaternion rotate, Transform parent) => Instantiate(prefab, position, rotate, parent);
        
    }
}
