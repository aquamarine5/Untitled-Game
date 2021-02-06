using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteChange : MonoBehaviour
{
    public Button button;
    public UIButton[] uIButtons;
    public void ChangeSprite(int index)
    {
        var spriteShate = new SpriteState
        {
            highlightedSprite = uIButtons[index].normal,
            pressedSprite = uIButtons[index].pressed,
            selectedSprite = uIButtons[index].pressed,
            disabledSprite = uIButtons[index].normal
        };
        button.spriteState = spriteShate;
    }
}
