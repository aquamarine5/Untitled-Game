using System.Collections;
using System.Collections.Generic;

public class WeaponBase
{
    public enum AttackType
    {
        Melee = 1
    }
    
    
}
class Test
{
    public delegate void DelegateWeaponAttack();
    public event DelegateWeaponAttack EventWeaponAttck;
}
class main
{
    void m()
    {
        Test test = new Test();
        test.EventWeaponAttck += a;
    }
    void a() { }
    
}