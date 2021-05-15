using UnityEngine;

public class PlayerMovementRB : MonoBehaviour
{
    public float turnMidAirMultiplier = 0.5f;
    public float speed = 1500f;
    public float maxSpeed = 800f;
    public float turnSpeed = 100f;
    public float jumpForce = 10000;
    public float tagDistance = 2f;
    public GameObject tagPrefab;
    public float maxPlayerAngle = 30f;
    Rigidbody RigidPlayerRb;

    bool isGrounded = true;

    GameObject ui;
    float DefaultDrag;

    float currentInclination = 0f;

    PanelBehaviour currentPanel = null;
    // Start is called before the first frame update
    void Start()
    {
        RigidPlayerRb = GetComponentInChildren<Rigidbody>();
        DefaultDrag = RigidPlayerRb.drag;
        RigidPlayerRb.transform.parent = null;
        ui = GameObject.FindGameObjectWithTag("UI");
    }

    void Update()
    {
        Vector2 mvtVelocity = new Vector2(RigidPlayerRb.velocity.x, RigidPlayerRb.velocity.y);
        if (Input.GetKeyDown("space") && isGrounded)
            RigidPlayerRb.AddForce(Vector3.up * jumpForce);

        transform.position = RigidPlayerRb.transform.position + new Vector3(0f, 0.5f, 0f);

        Debug.DrawRay(ui.transform.position, Vector3.down * 0.75f, Color.green, 5);
        // if (Input.GetAxis("Horizontal") < 0 && ui.transform.rotation.eulerAngles.x == 0 && ui.transform.rotation.eulerAngles.z == 0)
        //     ui.transform.RotateAround(ui.transform.position + Vector3.down, transform.forward, 45f);
        // else if (Input.GetAxis("Horizontal") > 0 && ui.transform.rotation.eulerAngles.x == 0 && ui.transform.rotation.eulerAngles.z == 0)
        //     ui.transform.RotateAround(ui.transform.position + Vector3.down, transform.forward, 45f * -1);
        // else if (Input.GetAxis("Horizontal") == 0 && ui.transform.rotation.eulerAngles.z != 0)
        // {
        //     if (ui.transform.rotation.eulerAngles.z < 0)
        //         ui.transform.RotateAround(ui.transform.position + Vector3.down, transform.forward, 45f);
        //     else
        //         ui.transform.RotateAround(ui.transform.position + Vector3.down, transform.forward, -45f);

        //     ui.transform.rotation = Quaternion.Euler(0, ui.transform.rotation.eulerAngles.y, 0);
        // }

        if (mvtVelocity.magnitude > maxSpeed)
        {
            RigidPlayerRb.velocity = Vector3.ClampMagnitude(RigidPlayerRb.velocity, maxSpeed);
        }
        if (RigidPlayerRb.velocity.y < 0)
        {
            RigidPlayerRb.velocity += Physics.gravity * Time.deltaTime;
        }
        UpdatePlayerFrontAngle();
        PanelCheck();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, LayerMask.NameToLayer("Player"));

        float turnFactor = isGrounded ? 1f : 1f * turnMidAirMultiplier;

        Quaternion cameraRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Input.GetAxis("Horizontal") * turnSpeed * turnFactor, 0f));
        Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            slopeRotation * cameraRotation,
            5 * Time.deltaTime
        );

        float forwardMvt = 0;
        if (isGrounded)
        {
            if (RigidPlayerRb.drag == 0)
                RigidPlayerRb.drag = DefaultDrag;
            forwardMvt = Input.GetAxis("Vertical");
        }
        else
        {
            RigidPlayerRb.drag = 0;
        }

        //Debug.DrawRay(transform.position, 1.5f * Vector3.down, Color.green, 5);
        RigidPlayerRb.AddForce(transform.forward * forwardMvt * speed);
    }

    void spray(RaycastHit hit) {
        if (Input.GetKeyDown("e")) {
            Vector3 between = Vector3.Normalize(hit.point - transform.position);
            print(between);
            Quaternion hitRotation = Quaternion.FromToRotation(Vector3.back, hit.normal);
            if (hitRotation.x != 0) {
                hitRotation = Quaternion.Euler(0, -180, 0);
            }
            Instantiate(tagPrefab,  new Vector3(hit.point.x - between.x, hit.point.y - between.y, hit.point.z - between.z), hitRotation);
        }
    }
    void PanelCheck()
    {
        LayerMask.GetMask("Panel");
        RaycastHit hitLeft;
        RaycastHit hitRight;
        bool collisionLeft = Physics.Raycast(transform.position, -transform.right, out hitLeft, tagDistance);
        bool collisionRight = Physics.Raycast(transform.position, transform.right, out hitRight, tagDistance);

        if (collisionLeft) {
            spray(hitLeft);
        }

        if (collisionRight) {
            spray(hitRight);
        }

        /// Prints ray in debug 
        Debug.DrawRay(ui.transform.position, -transform.right * tagDistance, Color.red, 5);
        Debug.DrawRay(ui.transform.position, transform.right * tagDistance, Color.blue, 5);
        if (currentPanel != null && Input.GetKeyDown("k"))
            currentPanel.Tag();
    }

    void UpdatePlayerFrontAngle()
    {
        float forwardMvt = Input.GetAxis("Vertical");
        float newAngle = forwardMvt * maxPlayerAngle;
        ui.transform.RotateAround(ui.transform.position + Vector3.down * 0.75f, transform.right, newAngle - currentInclination);
        ui.transform.localPosition = new Vector3(ui.transform.localPosition.x, 0, ui.transform.localPosition.z);
        currentInclination = newAngle;
    }

    public void PanelZoneEntered(PanelBehaviour panel)
    {
        currentPanel = panel;
    }

    public void PanelZoneExit()
    {
        currentPanel = null;
    }
}
