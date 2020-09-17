using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChange : MonoBehaviour
{
    public Sprite[] sprites;
    public WeaponBase.WeaponType Weapon;
    public Image buttonImage;
    public SpriteRenderer characterImage;
    public void OnButtonClick()
    {
        if ((int)Weapon == (sprites.Length-1)+100)
        {
            Weapon = (WeaponBase.WeaponType)100;
        }
        else
        {
            Weapon++;
        }
        buttonImage.sprite = sprites[(int)Weapon];
        characterImage.sprite = sprites[(int)Weapon];
    }
}
