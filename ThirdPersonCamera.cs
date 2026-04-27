using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;        // drag karakter ke sini

    [Header("Camera Settings")]
    public float distance     = 4f;
    public float height       = 2f;
    public float sensitivity  = 3f;
    public float minYAngle    = -20f;
    public float maxYAngle    =  60f;

    private float _yaw;   // rotasi horizontal
    private float _pitch; // rotasi vertical

    void Start()
    {
        _yaw   = transform.eulerAngles.y;
        _pitch = 15f;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Input mouse
        _yaw   += Input.GetAxis("Mouse X") * sensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        _pitch  = Mathf.Clamp(_pitch, minYAngle, maxYAngle);

        // Hitung posisi kamera
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3    offset   = rotation * new Vector3(0f, 0f, -distance);

        transform.position = target.position
                           + Vector3.up * height
                           + offset;
        transform.LookAt(target.position + Vector3.up * height);
    }
}