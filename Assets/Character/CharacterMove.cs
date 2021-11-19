using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private Transform groundChecker;

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private int mouseSensitivity = 100;

    [SerializeField]
    private float movementSpeed = 10f;

    [SerializeField]
    private float gravity = -9.81f;

    [SerializeField]
    private float jumpHeight = 2f;
    
    [SerializeField]
    private float leftControlMultiplierValue = 2f;

    [SerializeField]
    private float groundCheckDistance = 0.1f;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool isFreecam = false;

    Vector3 velocity;

    bool isGrounded;

    float xRotation = 0f, yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // lock cursor to screen on startup
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse position for camera/controller rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // get primary movement keys for controller movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // create a new vector to store movement vector
        Vector3 move;

        // dont do anything if the movement is too high
        // this is most often a result of the mouse movement going a little nuts when new chunks generate
        if (Mathf.Abs(mouseX) > 20 || Mathf.Abs(mouseY) > 20)
            return;

        // create a new xRotation variable for up/down movement
        xRotation -= mouseY;
        // clamp to 180deg so we don't over rotate
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        yRotation += mouseX;

        if (isFreecam)
        {
            // init velocity and control multiplier to values
            velocity.y = 0f;
            float ctrlMultiplier = 1f;

            // if jumping, move straight up
            if (Input.GetButton("Jump"))
                velocity.y = movementSpeed;
            // if shifting, move straight down
            if(Input.GetKey(KeyCode.LeftShift))
                velocity.y = -movementSpeed;

            // if left control is held, increase multiplier
            if (Input.GetKey(KeyCode.LeftControl))
                ctrlMultiplier = leftControlMultiplierValue;

            controller.Move(velocity * ctrlMultiplier * Time.deltaTime);

            // set move based on only vertical inputs axis (we move player
            // according to where it is looking on X & Y rotation axis, instead of just X)
            move = transform.forward * z * ctrlMultiplier;
            // reset camera rotation
            playerCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            // if freecam is enabled, we want to locally rotate the character so
            // Vector3.forward will move in the direction the player is looking
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
        else
        {
            // check if the player is grounded based on ground check distance
            isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckDistance, groundMask);

            if(isGrounded)
            {
                if(velocity.y < 0)
                {
                    // set the y velocity to something low (not 0, could cause issues)
                    velocity.y = -2f;
                    // we know the player is grounded, so we can set the slope
                    // limit to something normal to prevent weird issues
                    controller.slopeLimit = 45f;
                }

                if(Input.GetButtonDown("Jump"))
                {
                    // set velocity based on jump height
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    // set slope limite to something high to prevent
                    // weird issues when walking into walls & jumping
                    controller.slopeLimit = 90f;
                }
            }

            velocity.y += gravity * 2f * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            // set move based on x & z inputs
            move = transform.right * x + transform.forward * z;

            // if free cam is not enabled, we want to locally rotate the camera 
            // so Vector3.forward will not move the character in the direction
            // the player is looking, just along that axis
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // rotate the controller along the x axis
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        controller.Move(move * movementSpeed * Time.deltaTime);
    }
}
