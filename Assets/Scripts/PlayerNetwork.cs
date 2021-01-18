using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNetwork : NetworkBehaviour
{
    [Header("Local player argument")]
    public Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
    public PlayerMove playerMove;
    [Header("Self argument")]
    public new Rigidbody2D rigidbody;

    public override void OnStartLocalPlayer()
    {
        cinemachineVirtualCamera.Follow = transform;
        playerMove.rd2d = rigidbody;
    }
}
