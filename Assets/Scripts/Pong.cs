using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pong : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D rb2d;
    void Start()
    {
        rb2d.AddForce(new Vector2(25,50));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
