using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNetwork : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        NetworkControl.S.cinemachineVirtualCamera.Follow = transform;
        NetworkControl.S.playerMove.rd2d = GetComponent<Rigidbody2D>();
        
    }
}
