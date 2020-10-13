using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueScript : MonoBehaviour
{
    public WeaponBase weaponBaseScript;
    public BlockAsset blockAsset;
    public LanguageData languageData;
    public static WeaponBase.IWeapon convert(string i)
    {
        switch (i)
        {
            case "chalk":
                return new Chalk() { attackType = WeaponBase.AttackType.Melee };
            case "o":
                return null;
            default:
                return null;
        }
    }
    public static CatalogueScript ReturnThis()
    {
        return GameObject.FindGameObjectWithTag("Catalogue").GetComponent<CatalogueScript>();
    }
}
