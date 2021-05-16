using UnityEngine;

public class PlayerMovementRB : MonoBehaviour
{
    const float BASE_HOVER_PITCH = 1f;
    public float turnMidAirMultiplier = 0.5f;
    public float speed = 1500f;
    public float maxSpeed = 800f;
    public float turnSpeed = 100f;
    public float jumpForce = 10000;
    public float tagDistance = 2.5f;
    public float forwardTagDistance = 3.5f;
    [SerializeField]
    GameObject[] tagPrefabs;
    public float maxPlayerAngle = 30f;
    Rigidbody RigidPlayerRb;

    private Vector3 sprayOffset = new Vector3(0, 2, 0);

    bool isGrounded = true;

    GameObject ui;
    float DefaultDrag;

    float currentInclination = 0f;

    PanelBehaviour currentPanel = null;

    public float lvlSpeedMultiplier = 1f;
    public float lvlTurnMultiplier = 1f;
    AudioManager audioManager;

    public string[] voiceLines;
    public float maxTimeVoiceLine = 10f;
    public float minTimeVoiceLine = 2f;
    float timeUntilVoiceLine;
    bool canTilt = true;
    // Start is called before the first frame update
    void Start()
    {
        SetVoiceLineTimer();
        RigidPlayerRb = GetComponentInChildren<Rigidbody>();
        DefaultDrag = RigidPlayerRb.drag;
        RigidPlayerRb.transform.parent = null;
        ui = GameObject.FindGameObjectWithTag("UI");
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        audioManager.Play("Hover");
    }

    void Update()
    {
        Vector2 mvtVelocity = new Vector2(RigidPlayerRb.velocity.x, RigidPlayerRb.velocity.y);
        if (Input.GetKeyDown("space") && isGrounded)
            RigidPlayerRb.AddForce(Vector3.up * jumpForce);

        transform.position = RigidPlayerRb.transform.position + new Vector3(0f, 0.5f, 0f);

        Debug.DrawRay(ui.transform.position, Vector3.down * 0.75f, Color.green, 5);

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
        ManageVoiceLine();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateCanTilt();
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, LayerMask.NameToLayer("Player"));

        float turnFactor = isGrounded ? 1f : 1f * turnMidAirMultiplier;

        Quaternion cameraRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Input.GetAxis("Horizontal") * turnSpeed * turnFactor * lvlTurnMultiplier, 0f));
        Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);

        RigidPlayerRb.rotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 90);
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
        RigidPlayerRb.AddForce(transform.forward * forwardMvt * speed * lvlSpeedMultiplier);
    }

    void spray(RaycastHit hit)
    {
        Quaternion hitRotation = Quaternion.LookRotation(-hit.normal);
        int tagIndex = Random.Range(0, tagPrefabs.Length);
        Instantiate(tagPrefabs[tagIndex], hit.point + (hit.normal * 0.5f), hitRotation);
        audioManager.Play("Spray");
    }
    void PanelCheck()
    {
        RaycastHit hitLeft;
        RaycastHit hitRight;
        RaycastHit hitForward;
        bool collisionForward = Physics.Raycast(transform.position + sprayOffset, transform.forward, out hitForward, forwardTagDistance);
        bool collisionLeft = Physics.Raycast(transform.position + sprayOffset, -transform.right, out hitLeft, tagDistance);
        bool collisionRight = Physics.Raycast(transform.position + sprayOffset, transform.right, out hitRight, tagDistance);
        if (currentPanel == null && getInteractionInput())
        {
            if (collisionForward)
            {
                spray(hitForward);
            }
            else if (collisionLeft)
            {
                spray(hitLeft);
            }
            else if (collisionRight)
            {
                spray(hitRight);
            }
        }
        else if (getInteractionInput())
        {
            audioManager.Play("SprayObjectif");
            currentPanel.Tag();
        }
        /// Prints ray in debug 
        Debug.DrawRay(ui.transform.position + sprayOffset, -transform.right * tagDistance, Color.red, 5);
        Debug.DrawRay(ui.transform.position + sprayOffset, transform.right * tagDistance, Color.blue, 5);
    }

    void UpdatePlayerFrontAngle()
    {
        if (canTilt)
        {
            float forwardMvt = Input.GetAxis("Vertical");
            float newAngle = forwardMvt * maxPlayerAngle;
            ui.transform.RotateAround(ui.transform.position + Vector3.down * 0.75f, transform.right, newAngle - currentInclination);
            ui.transform.localPosition = new Vector3(ui.transform.localPosition.x, 0, ui.transform.localPosition.z);
            currentInclination = newAngle;
            UpdateHoverSound(forwardMvt);
        }
    }

    void UpdateHoverSound(float playerInput)
    {
        audioManager.ChangeSoundParam("Hover", "pitch", Mathf.Abs(playerInput) * 1f + BASE_HOVER_PITCH);
    }
    public void PanelZoneEntered(PanelBehaviour panel)
    {
        currentPanel = panel;
    }

    public void PanelZoneExit()
    {
        currentPanel = null;
    }

    private bool getInteractionInput()
    {
        return Input.GetKeyDown("e");
    }
    void UpdateCanTilt()
    {
        if (Physics.Raycast(transform.position, transform.forward, 2f, LayerMask.NameToLayer("Player")))
        {
            this.canTilt = false;
            float newAngle = 0;
            ui.transform.RotateAround(ui.transform.position + Vector3.down * 0.75f, transform.right, newAngle - currentInclination);
            ui.transform.localPosition = new Vector3(ui.transform.localPosition.x, 0, ui.transform.localPosition.z);
            currentInclination = newAngle;
        }
        else
            this.canTilt = true;
    }

    void ManageVoiceLine()
    {
        timeUntilVoiceLine -= Time.deltaTime;
        if (timeUntilVoiceLine < 0)
        {
            audioManager.Play(ChooseRandomVoiceLine());
            SetVoiceLineTimer();
        }
    }
    string ChooseRandomVoiceLine()
    {
        int voiceLineIndex = Random.Range(0, voiceLines.Length);
        return voiceLines[voiceLineIndex];
    }

    void SetVoiceLineTimer()
    {
        timeUntilVoiceLine = Random.Range(minTimeVoiceLine, maxTimeVoiceLine);
    }
}
