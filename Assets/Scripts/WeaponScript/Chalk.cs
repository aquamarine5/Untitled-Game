using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chalk : WeaponBase.IWeapon
{
    public WeaponBase.AttackType attackType { get; set; }
    public Chalk(WeaponBase.AttackType attackType)
    {
        SetValue(attackType);
    }
    public void OnAttack()
    {
        
    }

    public void OnChange()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void SetValue(WeaponBase.AttackType attack)
    {
        attackType = attack;
    }
}
