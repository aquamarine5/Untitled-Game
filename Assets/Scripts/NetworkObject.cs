using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkObject : MonoBehaviour
{
    [Header("UI")] 
    public Text text;
    public Image networkStatusImage;
    [Header("Network Information")]
    [Tooltip("Is the version correctly equal?")]public bool isVersionCorrectly;
    public string ip;
    public string objectName;
    float pingCD;
    Ping ping;
    public void UpdateUI()
    {
        text.text = $"{objectName}（{ip}）";
        networkStatusImage.sprite = NetworkRxDiscover.S.none;
    }
    private void Update()
    {
        if (ip == null) return;
        if (ip == "") return;

        if (ping == null && pingCD <= 0f) 
        {
            ping = new Ping(ip);
            pingCD = 10f;
        }
        else
        {
            pingCD -= Time.deltaTime;
            if (ping.isDone)
            {
                if (ping.time < 50) networkStatusImage.sprite = NetworkRxDiscover.S.good;
                else if (ping.time < 200) networkStatusImage.sprite = NetworkRxDiscover.S.middle;
                else if (ping.time < 500) networkStatusImage.sprite = NetworkRxDiscover.S.bad;
                else networkStatusImage.sprite = NetworkRxDiscover.S.none;
                ping.DestroyPing();
                ping = null;
            }
        }
        
    }
}
