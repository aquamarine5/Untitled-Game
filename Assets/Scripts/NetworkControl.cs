using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkControl : NetworkManager
{
    [Header("Player require argument")]
    public Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
    public PlayerMove playerMove;
    [Space(10)]
    public GameObject localPlayer;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab, GetStartPosition().position, new Quaternion());
        NetworkServer.AddPlayerForConnection(conn, player);
        if (numPlayers == 1) { 
            localPlayer = player;
            cinemachineVirtualCamera.Follow = localPlayer.transform;
            playerMove.rd2d = localPlayer.GetComponent<Rigidbody2D>();
        }
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }
}
