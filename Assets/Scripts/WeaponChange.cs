﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class WeaponChange : MonoBehaviour
{
    public Sprite[] sprites;
    public WeaponType Weapon;
    public Image buttonImage;
    public WeaponAsset[] weaponAsset;
    public SpriteRenderer characterImage;
    public void OnButtonClick()
    {
        if ((int)Weapon == (sprites.Length-1)+100)
        {
            Weapon = (WeaponType)100;
        }
        else
        {
            Weapon++;
        }
        buttonImage.sprite = sprites[(int)Weapon];
        characterImage.sprite = sprites[(int)Weapon];
    }
}
