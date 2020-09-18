using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour,WeaponBase.IWeapon
{
    public Sprite[] WeaponScripts;
    public Object[] WeaponBaseScripts;
    public Object NowWeapon;
    public enum AttackType
    {
        Base = -1,
        Melee = 1,
        Remote = 2
    }
    public enum WeaponType
    {
        Chalk = 100,
        ColorPen_Black = 101,
        ColorPen_Red = 102,
        ColorPen_Blue = 103,
    }
    public void Attack()
    {
        //EventWeaponAttck?.Invoke();
    }
    public AttackType attackType { get; set; }

    public interface IWeapon
    {
        AttackType attackType { get; set; }
        void OnAttack();
        void OnChange();
        void OnUpdate();
    }

    public void OnAttackClick()
    {
        ((IWeapon)NowWeapon).OnAttack();
    }
    public void OnChangeClick()
    {
        ((IWeapon)NowWeapon).OnChange();
    }
    void Update()
    {
        ((IWeapon)NowWeapon).OnUpdate();
    }
    void IWeapon.OnAttack(){ }
    void IWeapon.OnChange(){ }
    void IWeapon.OnUpdate(){ }
}