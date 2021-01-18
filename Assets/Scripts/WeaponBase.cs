using UnityEngine;
using Mirror;

public class WeaponBase : NetworkBehaviour
{
    public Sprite[] WeaponScripts;
    public WeaponAsset[] weaponAsset;
    public WeaponAsset nowWeaponAsset;
}
public abstract class WeaponInterface
{
    public abstract void OnAttack();
    public abstract void OnChange();
    public abstract void OnUpdate();
    public AttackType attackType;
}
public enum AttackType
{
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