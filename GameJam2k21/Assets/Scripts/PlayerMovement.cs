using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{

    public float maxSpeed = 8f;
    public float timeZeroToMax = 2.5f;

    public float forwardVelocity = 0f;
    public float rightVelocity = 0f;
    float accelRate;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        accelRate = maxSpeed / timeZeroToMax;
        rotation.y = transform.eulerAngles.y;
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        forward.y = 0;
        Vector3 right = transform.TransformDirection(Vector3.right);
        right.y = 0;

        float verticalAxis = Input.GetAxis("Vertical");
        if (verticalAxis != 0)
        {
            forwardVelocity += accelRate * verticalAxis * Time.deltaTime;
            forwardVelocity = forwardVelocity >= 0
                ? Mathf.Min(forwardVelocity, maxSpeed)
                : Mathf.Max(forwardVelocity, -maxSpeed);
        }
        else
        {
            forwardVelocity = forwardVelocity > 0
            ? forwardVelocity - (accelRate * Time.deltaTime)
            : forwardVelocity + (accelRate * Time.deltaTime);
        }

        float horizontalAxis = Input.GetAxis("Horizontal");

        // rotation.y += horizontalAxis * lookSpeed;

        if (horizontalAxis != 0)
        {
            rightVelocity += accelRate * horizontalAxis * Time.deltaTime;
            rightVelocity = rightVelocity > 0
                ? Mathf.Min(rightVelocity, maxSpeed)
                : Mathf.Max(rightVelocity, -maxSpeed);
        }
        else
        {
            rightVelocity = rightVelocity > 0
            ? rightVelocity - (accelRate * Time.deltaTime)
            : rightVelocity + (accelRate * Time.deltaTime);
        }

        if (characterController.isGrounded)
        {
            moveDirection = (forward * forwardVelocity) + (right * rightVelocity);
            moveDirection.y = 0;

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else
        {
            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            moveDirection.y -= gravity * Time.deltaTime;
        }


        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }
}