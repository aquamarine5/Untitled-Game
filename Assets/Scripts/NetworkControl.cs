using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class NetworkControl : NetworkManager
{
    public static NetworkControl S { get; private set; }

    [Header("Player Require Argument")]
    public Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
    public PlayerMove playerMove;

    [Space(10)]
    public GameObject localPlayer;
    public override void Awake()
    {
        S = this;
        base.Awake();
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab, GetStartPosition().position, new Quaternion());
        NetworkServer.AddPlayerForConnection(conn, player);
        if (conn.identity.isLocalPlayer) { 
            localPlayer = player;
        }

    }
    private string GetIP()
    {
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adater in adapters)
        {
            if (adater.Supports(NetworkInterfaceComponent.IPv4))
            {
                UnicastIPAddressInformationCollection UniCast = adater.GetIPProperties().UnicastAddresses;
                if (UniCast.Count > 0)
                {
                    foreach (UnicastIPAddressInformation uni in UniCast)
                    {
                        if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            //Debug.Log(uni.Address.ToString());
                            return uni.Address.ToString();
                        }
                    }
                }
            }
        }
        return null;
    }
    public static string GetLocalIP()
    {
        try
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress item in IpEntry.AddressList)
            {
                //AddressFamily.InterNetwork  ipv4
                //AddressFamily.InterNetworkV6 ipv6
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    return item.ToString();
                }
            }
            return "";
        }
        catch { return ""; }
    }
}
