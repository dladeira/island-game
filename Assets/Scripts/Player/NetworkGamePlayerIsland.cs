using UnityEngine;
using Mirror;

public class NetworkGamePlayerIsland : NetworkBehaviour
{
    [SyncVar] public string displayName;

    [SerializeField] private float accel;
    [SerializeField] private float friction;
    [SerializeField] private float jumpForce;

    private float jumpTime = -1;
    private float jumpDuration = 0.1f;

    private Rigidbody rb;

    void Start()
    {
        DontDestroyOnLoad(this);
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        Vector3 playerVelocity = rb.velocity;

        playerVelocity = CalculateFriction(playerVelocity);
        playerVelocity += CalculateMovement(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), playerVelocity);

        rb.velocity = playerVelocity;
    }

    private Vector3 CalculateFriction(Vector3 currentVelocity)
    {
        float speed = currentVelocity.magnitude;

        if (!CheckGround() || Input.GetKeyDown(KeyCode.Space) || speed == 0) return currentVelocity;

        float drop = speed * friction * Time.deltaTime;
        return currentVelocity * (Mathf.Max(speed - drop, 0f) / speed);
    }

    private Vector3 CalculateMovement(Vector2 input, Vector3 currentVelocity)
    {

        if (!CheckGround())
        {
            return Vector3.zero;
        }

        Vector3 inputVelocity = Quaternion.Euler(transform.eulerAngles) * new Vector3(input.x * accel, 0f, input.y * accel);
        inputVelocity += CalculateJump(inputVelocity.y);

        return inputVelocity;
    }

    private Vector3 CalculateJump(float yVelocity)
    {
        if (Time.time < jumpTime + jumpDuration && yVelocity < jumpForce && CheckGround())
        {
            Debug.Log(Vector3.up * jumpForce);
            return Vector3.up * jumpForce;
        }

        return Vector3.zero;
    }

    private bool CheckGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        return Physics.Raycast(ray, GetComponent<Collider>().bounds.extents.y + 0.1f);
    }
}
