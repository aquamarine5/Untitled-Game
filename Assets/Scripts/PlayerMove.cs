using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public MoveScripts moveScripts;
    public GameObject dieUI;
    public GameObject player;
    new Rigidbody rigidbody;
    int floor;
    readonly float rayLength = 200f;
    Ray ray;
    Animator anim;
    private void Start()
    {
        rigidbody = player.GetComponent<Rigidbody>();
        floor = LayerMask.GetMask("Floor");
        anim = player.GetComponent<Animator>();
    }

    private void Update()
    {
        //Check();
        MoveStick();
        //Rotate();
        //Animating();
    }
    private void Check()
    {
        if (player.transform.position.y <= -20)
        {
            dieUI.SetActive(true);
            Time.timeScale = 0;
        }
    }
    private void MoveStick()
    {
        player.transform.Translate(new Vector3(
            moveScripts.vector3.x,
            moveScripts.vector3.y,
            0) * speed * Time.deltaTime);
    }
    private void Animating()
    {
        bool ismoving;
        ismoving = true;
        anim.SetBool("isMoving", ismoving);
    }
    private void Rotate()
    {
        if (Input.touches.Length > 0)
        {
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, rayLength, floor))
            {
                Vector3 vector = raycastHit.point - player.transform.position;
                Quaternion quaternion = Quaternion.LookRotation(vector);
                rigidbody.MoveRotation(quaternion);
            }
        }
    }
}
