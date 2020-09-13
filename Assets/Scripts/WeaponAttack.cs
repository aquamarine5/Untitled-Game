using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    public AttackType at;
    public RectTransform SpriteTf;
    public enum AttackType
    {
        Melee=1
    }
    public void OnButtonClick()
    {

    }
}
