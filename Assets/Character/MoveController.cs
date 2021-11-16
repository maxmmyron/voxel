using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private Transform groundChecker;

    [SerializeField]
    private float movementSpeed = 10f;

    [SerializeField]
    private float gravity = -9.81f;

    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private float groundCheckDistance = 0.1f;

    [SerializeField]
    private LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            controller.slopeLimit = 45f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z; // relative movement based on direction player is moving

        controller.Move(move * movementSpeed * Time.deltaTime);
        
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            controller.slopeLimit = 90f;
        }
        
        velocity.y += gravity * 2f * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
