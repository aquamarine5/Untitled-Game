using System.Collections;
using System.Collections.Generic;

public class WeaponBase
{
    public delegate void DelegateWeaponAttack();
    public event DelegateWeaponAttack EventWeaponAttck;
    public enum AttackType
    {
        Melee = 1
    }
    public void Attack()
    {
        EventWeaponAttck?.Invoke();
    }
    interface IWeapon
    {
        public void OnChange();
    }
}