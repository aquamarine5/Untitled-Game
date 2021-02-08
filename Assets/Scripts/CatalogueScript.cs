using UnityEngine;
using UnityEngine.UI;

public class CatalogueScript : MonoBehaviour
{
    public static CatalogueScript S;
    [Tooltip("显示Seed的UI.Text")] public Text seedText;
    public WeaponBase weaponBaseScript;
    public BlockAsset blockAsset;
    public LanguageData languageData;
    public XLuaControl xLuaScript;

    private void Awake()
    {
        S = this;
        BlockAsset.BlockAssetInstance = blockAsset;
        LanguageLibrary.LanguageDataInstance = languageData;
        BlockLibrary.BlockAssetInstance = blockAsset;
    }
}
