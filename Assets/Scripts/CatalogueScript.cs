using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueScript : MonoBehaviour
{
    public WeaponBase weaponBaseScript;
    //public WeaponAsset wa;
    private void Start()
    {
        //Debug.Log(wa.wbScript.ToString());
    }
    public static WeaponBase.IWeapon convert(string i)
    {
        switch (i)
        {
            case "chalk":
                return new Chalk(WeaponBase.AttackType.Melee);
            case "o":
                return null;
            default:
                return null;
        }
    }
}
