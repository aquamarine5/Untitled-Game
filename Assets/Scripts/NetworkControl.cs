using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkControl : NetworkManager
{
    /// <summary>
    /// If the network environment is WIFI return true.<br/>
    /// <seealso cref="Application.internetReachability"/>
    /// </summary>
    public static bool isNetworkAvailable => Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
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
    public override void OnStartHost()
    {
        NetworkRxDiscover.S.AdvertiseServer();
        if(isNetworkAvailable)
            base.OnStartHost();
        else
        {
            Debug.LogError("FAILED");
        }
    }
    public override void OnStartClient()
    {
        if(isNetworkAvailable)
            base.OnStartClient();
        else
        {
            Debug.LogError("FAILED");
        }
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab, GetStartPosition().position, new Quaternion());
        NetworkServer.AddPlayerForConnection(conn, player);
        if (conn.identity.isLocalPlayer) { 
            localPlayer = player;
        }
    }
}

public enum NetworkPingStatus
{
    /// <summary>
    /// <see cref="NetworkRxDiscover.bad"/>
    /// </summary>
    Bad = 0,
    /// <summary>
    /// <see cref="NetworkRxDiscover.middle"/>
    /// </summary>
    Middle = 1,
    /// <summary>
    /// <see cref="NetworkRxDiscover.good"/>
    /// </summary>
    Good = 2,
    /// <summary>
    /// <see cref="NetworkRxDiscover.none"/>
    /// </summary>
    None = 100
}
