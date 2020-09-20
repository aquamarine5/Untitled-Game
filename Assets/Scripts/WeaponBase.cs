using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour,WeaponBase.IWeapon
{
    public Sprite[] WeaponScripts;
    public WeaponAsset[] weaponAsset;
    public WeaponAsset nowWeaponAsset;
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
        //public float a { get; set; }
        AttackType attackType { get; set; }
        void SetValue(AttackType attack);
        void OnAttack();
        void OnChange();
        void OnUpdate();
    }

    public void OnAttackClick()
    {
        ((IWeapon)nowWeaponAsset).OnAttack();
    }
    public void OnChangeClick()
    {
        ((IWeapon)nowWeaponAsset).OnChange();
    }
    void Update()
    {
        //((IWeapon)nowWeaponAsset.wbScript.GetClass()).OnUpdate();
    }
    void IWeapon.OnAttack(){ }
    void IWeapon.OnChange(){ }
    void IWeapon.OnUpdate(){ }

    public void SetValue(AttackType attack)
    {
        throw new System.NotImplementedException();
    }
}