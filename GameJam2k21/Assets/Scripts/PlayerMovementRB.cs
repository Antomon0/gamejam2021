using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour
{
    const float speed = 1500f;
    const float maxSpeed = 600f;
    Rigidbody RigidPlayerRb;

    GameObject ui;

    float DefaultDrag;
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
        if (Input.GetKeyDown("space"))
            RigidPlayerRb.AddForce(Vector3.up * 16000f);

        transform.position = RigidPlayerRb.transform.position + new Vector3(0f, 0.5f, 0f);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, Input.GetAxis("Horizontal") * Time.deltaTime * 100f, 0f));
        Debug.DrawRay(ui.transform.position, Vector3.down, Color.green, 5);
        if (Input.GetAxis("Horizontal") < 0 && ui.transform.rotation.eulerAngles.x == 0 && ui.transform.rotation.eulerAngles.z == 0)
            ui.transform.RotateAround(ui.transform.position + Vector3.down, transform.forward, 45f);
        else if (Input.GetAxis("Horizontal") > 0 && ui.transform.rotation.eulerAngles.x == 0 && ui.transform.rotation.eulerAngles.z == 0)
            ui.transform.RotateAround(ui.transform.position + Vector3.down, transform.forward, 45f * -1);
        else if (Input.GetAxis("Horizontal") == 0 && ui.transform.rotation.eulerAngles.z != 0)
        {
            if (ui.transform.rotation.eulerAngles.z < 0)
                ui.transform.RotateAround(ui.transform.position + Vector3.down, transform.forward, 45f);
            else
                ui.transform.RotateAround(ui.transform.position + Vector3.down, transform.forward, -45f);
        }

        if (RigidPlayerRb.velocity.magnitude > maxSpeed)
        {
            RigidPlayerRb.velocity = Vector3.ClampMagnitude(RigidPlayerRb.velocity, maxSpeed);
        }
        if (RigidPlayerRb.velocity.y < 0)
        {
            RigidPlayerRb.velocity += Physics.gravity * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        float forwardMvt = 0;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, LayerMask.NameToLayer("Player")))
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
        float sideMvt = Input.GetAxis("Horizontal");

        RigidPlayerRb.AddForce(transform.forward * forwardMvt * speed);
    }
}
