using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MultiPlay : MonoBehaviour
{
    public UIButton[] multiPlaySprite;
    public Button button;
    public Image image;
    public enum NetworkPingStatus
    {
        Bad=0,
        Middle=1,
        Good=2
    }
    private void Update()
    {
        int ping = (int)NetworkTime.rtt * 1000;
        print(ping);
        if (0 < ping & ping < 200) SetSprite(NetworkPingStatus.Good);
        else if (200 < ping & ping < 1000) SetSprite(NetworkPingStatus.Middle);
        else if (1000 > ping) SetSprite(NetworkPingStatus.Bad);

    }
    void SetSprite(NetworkPingStatus networkPingStatus)
    {
        UIButton uiButton = multiPlaySprite[(int)networkPingStatus];
        var state = button.spriteState;
        image.sprite = uiButton.normal;
        state.highlightedSprite = uiButton.normal;
        state.pressedSprite = uiButton.pressed;
        state.selectedSprite = uiButton.pressed;
        state.disabledSprite = uiButton.normal;
    }
}
[System.Serializable]
public struct UIButton
{
    public Sprite normal;
    public Sprite pressed;
}