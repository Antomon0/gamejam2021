using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{

    public float maxForwardSpeed = 8f;

    public float maxRightSpeed = 4f;
    public float timeZeroToMax = 0.5f;

    public float forwardVelocity = 0f;
    public float rightVelocity = 0f;

    float accelRateForward;
    float accelRateRight;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Transform playerCameraParent;
    public float lookSpeed = 0.3f;

    public float timeToTurn360 = 5;
    public float lookXLimit = 60.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        accelRateForward = maxForwardSpeed / timeZeroToMax;
        accelRateRight = maxRightSpeed / timeZeroToMax;
        rotation.y = transform.eulerAngles.y;
    }

    void Update()
    {

        Vector3 down = transform.TransformDirection(Vector3.down);
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, down, out hit, 2, layerMask))
        {
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal), 1 * Time.deltaTime);
            Debug.Log(Quaternion.FromToRotation(transform.up, hit.normal));
        }

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float verticalAxis = Input.GetAxis("Vertical");
        if (verticalAxis != 0)
        {
            forwardVelocity += accelRateForward * verticalAxis * Time.deltaTime;
            forwardVelocity = forwardVelocity >= 0
                ? Mathf.Min(forwardVelocity, maxForwardSpeed)
                : Mathf.Max(forwardVelocity, -maxForwardSpeed);
        }
        else
        {
            forwardVelocity = forwardVelocity > 0
            ? forwardVelocity - (accelRateForward * Time.deltaTime)
            : forwardVelocity + (accelRateForward * Time.deltaTime);
        }

        float horizontalAxis = Input.GetAxis("Horizontal");

        transform.RotateAround(transform.position, Vector3.up, horizontalAxis * lookSpeed);

        if (horizontalAxis != 0)
        {
            rightVelocity += accelRateRight * horizontalAxis * Time.deltaTime;
            rightVelocity = rightVelocity > 0
                ? Mathf.Min(rightVelocity, maxRightSpeed)
                : Mathf.Max(rightVelocity, -maxRightSpeed);
        }
        else
        {
            rightVelocity = rightVelocity > 0
            ? rightVelocity - (accelRateRight * Time.deltaTime)
            : rightVelocity + (accelRateRight * Time.deltaTime);
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
            // rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            // rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            // rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            // transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }
}