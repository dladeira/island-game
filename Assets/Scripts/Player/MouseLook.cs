using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public float sens = 10F;


    private float minY = -90F;
    private float maxY = 90F;

    float rotationY = 0F;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }
        float rotationX = player.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sens;

        rotationY += Input.GetAxis("Mouse Y") * sens;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        transform.localEulerAngles = new Vector3( -rotationY, 0, 0);
        player.transform.localEulerAngles = new Vector3(0, rotationX, 0);
    }
}