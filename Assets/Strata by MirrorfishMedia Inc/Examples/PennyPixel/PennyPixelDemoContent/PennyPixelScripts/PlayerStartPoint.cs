using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PennyPixel
{
    public class PlayerStartPoint : MonoBehaviour
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
