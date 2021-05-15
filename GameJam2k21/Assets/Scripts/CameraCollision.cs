using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform referenceTransform;
    public float collisionOffset = 0.2f; //To prevent Camera from clipping through Objects

    Vector3 defaultPos;
    Vector3 directionNormalized;
    Transform parentTransform;
    float defaultDistance;
    Camera cam;
    public float mouseUpDownAngle = 30f;
    public float mouseRightLeftAngle = 15f;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>();
        defaultPos = transform.localPosition;
        directionNormalized = defaultPos.normalized;
        parentTransform = transform.parent;
        defaultDistance = Vector3.Distance(defaultPos, Vector3.zero);
    }

    // FixedUpdate for physics calculations
    void FixedUpdate()
    {
        Vector3 currentPos = defaultPos;
        RaycastHit hit;
        Vector3 dirTmp = parentTransform.TransformPoint(defaultPos) - referenceTransform.position;
        if (Physics.SphereCast(referenceTransform.position, collisionOffset, dirTmp, out hit, defaultDistance))
        {
            currentPos = (directionNormalized * (hit.distance - collisionOffset));
        }

        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;
        Quaternion mouseRotation = Quaternion.Euler(new Vector4(-1f * (mouseY * mouseUpDownAngle), mouseX * mouseRightLeftAngle, transform.localRotation.z));
        cam.transform.localRotation = Quaternion.Slerp(
            cam.transform.localRotation,
            mouseRotation,
            5 * Time.deltaTime
        );

        transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * 5f);
    }
}