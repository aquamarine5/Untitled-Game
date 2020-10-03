using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class WeaponAsset : ScriptableObject
{
    public Object wbScript = null;
    public Object weaponScript = null;
    public Sprite[] weaponSprite;
    public float rotate;
    public Sprite[] attackSprite;
}
