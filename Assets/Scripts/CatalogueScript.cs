using UnityEngine;
using UnityEngine.UI;

public class CatalogueScript : MonoBehaviour
{
    public static BlockAsset BlockAssetInstance => BlockLibrary.BlockAssetInstance;
    public static LanguageData LanguageDataInstance => LanguageLibrary.LanguageDataInstance;
    public static PanelControl PanelInstance => PanelControl.PanelInstance;
    public static CatalogueScript CatalogueInstance;

    [Header("Tilemap asset")]
    public GameObject tilemapChunkMaster;
    public GameObject tilemapChunkPrefab;

    [Header("Static asset")]
    public Text seedText;

    [Header("ScriptableObject asset")]
    public WeaponBase weaponBaseScript;
    public BlockAsset blockAsset;
    public LanguageData languageData;
    public XLuaControl xLuaScript;

    private void Awake()
    {
        CatalogueInstance = this;
        LanguageLibrary.LanguageDataInstance = languageData;
        BlockLibrary.BlockAssetInstance = blockAsset;
    }
}
