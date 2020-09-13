using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChange : MonoBehaviour
{
    public Sprite[] sprites;
    public int Weapon;
    public Image buttonImage;
    public SpriteRenderer characterImage;
    public void OnButtonClick()
    {
        if (Weapon == sprites.Length-1)
        {
            Weapon = 0;
        }
        else
        {
            Weapon++;
        }
        buttonImage.sprite = sprites[Weapon];
        characterImage.sprite = sprites[Weapon];
    }
}
