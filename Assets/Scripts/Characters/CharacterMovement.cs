using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    CharacterController controller;
    protected float movementSpeed = 500.0f;
    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Rigidbody characterRB = GetComponent<Rigidbody>();
        Vector3 speed = (((transform.forward * controller.xAxis) + (transform.right * controller.zAxis)) * movementSpeed * Time.fixedDeltaTime);
        characterRB.velocity = new Vector3(speed.x, characterRB.velocity.y, speed.z);
    }
}
