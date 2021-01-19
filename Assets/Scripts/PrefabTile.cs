using Mirror;

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
        public void Instantiate(Transform parent)
        {
            GameObject gameObject =Instantiate(prefab, parent);
            NetworkServer.Spawn(gameObject);
        }
        public void Instantiate(Vector3 position, Quaternion rotate)
        {
            GameObject gameObject = Instantiate(prefab, position, rotate);
            NetworkServer.Spawn(gameObject);
        }

        public void Instantiate(Vector3 position, Quaternion rotate, Transform parent)
        {
            GameObject gameObject = Instantiate(prefab, position, rotate, parent);
            NetworkServer.Spawn(gameObject);
        }
    }
}
