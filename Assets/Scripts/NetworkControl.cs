using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
}
