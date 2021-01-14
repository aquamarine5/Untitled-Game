using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogueScript : MonoBehaviour
{
    [Tooltip("显示Seed的UI.Text")] public Text seedText;
    public WeaponBase weaponBaseScript;
    public BlockAsset blockAsset;
    public LanguageData languageData;
    public XLuaControl xLuaScript;

    public static CatalogueScript ReturnThis()
    {
        return GameObject.FindGameObjectWithTag("Catalogue").GetComponent<CatalogueScript>();
    }
}
