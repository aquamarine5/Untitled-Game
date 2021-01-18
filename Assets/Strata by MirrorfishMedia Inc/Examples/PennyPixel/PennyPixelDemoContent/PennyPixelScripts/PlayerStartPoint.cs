using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace PennyPixel
{
    public class PlayerStartPoint : NetworkBehaviour
    {
        Transform playerTransform;

        private void OnEnable()
        {
            playerTransform = GameObject.FindObjectOfType<PlayerPlatformerController>().transform;
           
        }

        private void Start()
        {
            playerTransform.position = this.transform.position;
        }

    }
}
