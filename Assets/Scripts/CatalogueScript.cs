using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueScript : MonoBehaviour
{
    public WeaponBase weaponBaseScript;
    public BlockAsset blockAsset;
    public LanguageData languageData;
    public XLuaControl xLuaScript;

    public static CatalogueScript ReturnThis()
    {
        return GameObject.FindGameObjectWithTag("Catalogue").GetComponent<CatalogueScript>();
    }
}
