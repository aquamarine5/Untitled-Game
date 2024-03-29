﻿using UnityEngine;
using Mirror;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody2D rd2d;
    public float speed;

    public void OnMove(Vector2 v)
    {
        rd2d.velocity = v * speed;
    }
    public void OnMoveEnd()
    {
        rd2d.velocity = new Vector2(0, 0);
    }
}