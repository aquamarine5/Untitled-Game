using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Net;
using System.Net.Sockets;

public class NetworkControl : NetworkManager
{
    public static NetworkControl S { get; private set; }

    [Header("Player Require Argument")]
    public Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
    public PlayerMove playerMove;
    [Header("UI")]
    public GameObject NetworkPanel;
    public Text ipText;
    [Space(10)]
    public GameObject localPlayer;
    public override void Awake()
    {
        S = this;
        base.Awake();
    }
    public override void Start()
    {
        ipText.text = $"SSID:{GetWifiSSID()}  IP:";
        base.Start();
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab, GetStartPosition().position, new Quaternion());
        NetworkServer.AddPlayerForConnection(conn, player);
        if (conn.identity.isLocalPlayer) { 
            localPlayer = player;
        }
    }
    public static string GetWifiSSID()
    {
#if UNITY_ANDROID
        AndroidJavaClass main = new AndroidJavaClass("com.syz.unitygamePlugin.Main");
        return main.Call<string>("GetNowWifiSSID");
#else
        return "";
#endif
    }
}
