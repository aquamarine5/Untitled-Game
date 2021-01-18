using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraNetwork : NetworkBehaviour
{
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public override void OnStartLocalPlayer()
    {
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                virtualCamera.Follow = gameObject.transform;
            }
        }
    }
}
