using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkGamePlayerIsland : NetworkBehaviour
{
    [SyncVar] public string displayName;

    [Scene] [SerializeField] private string lobbyScene;
    [SerializeField] private float accel;
    [SerializeField] private float friction;
    [SerializeField] private float jumpForce;

    [SerializeField] public float sens = 10F;
    [SerializeField] private Transform playerCamera;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;

    private float minY = -90F;
    private float maxY = 90F;

    float rotationY = 0F;

    private bool pressedJump = false;
    private float timeSinceJump = 0;

    [SerializeField] private Transform vaultLoc1;
    [SerializeField] private Transform vaultLoc2;
    [SerializeField] private float vL1Distance;
    [SerializeField] private float vL2Distance;
    [SerializeField] private float vaultSpeed;
    [SerializeField] private float vaultVerticalSpeed;

    private Rigidbody rb;

    void Start()
    {
        DontDestroyOnLoad(this);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        playerCamera.gameObject.SetActive(hasAuthority);

        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                pressedJump = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
            }

            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sens;

            rotationY += Input.GetAxis("Mouse Y") * sens;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);

            playerCamera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
        }
    }

    void FixedUpdate()
    {
        timeSinceJump += Time.fixedDeltaTime;

        animator.SetBool("IsRunning", rb.velocity.magnitude > 0.5);

        if (hasAuthority)
        {
            Vector3 playerVelocity = rb.velocity;

            playerVelocity = CalculateFriction(playerVelocity);
            playerVelocity += CalculateMovement(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), playerVelocity);

            rb.velocity = playerVelocity;
        }
    }

    void OnEnable()
    {
        NetworkManagerIsland.OnClientDisconnected += ReturnToMainMenu;
    }

    void OnDisable()
    {
        NetworkManagerIsland.OnClientDisconnected -= ReturnToMainMenu;
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene(lobbyScene);
    }

    private Vector3 CalculateFriction(Vector3 currentVelocity)
    {
        float speed = currentVelocity.magnitude;

        if (!CheckGround() || Input.GetKeyDown(KeyCode.Space) || speed == 0) return currentVelocity * (1 - (0.1f * Time.fixedDeltaTime));

        float drop = speed * friction * Time.deltaTime;
        return currentVelocity * (Mathf.Max(speed - drop, 0f) / speed);
    }

    private Vector3 CalculateMovement(Vector2 input, Vector3 currentVelocity)
    {
        Vector3 inputVelocity = new Vector3(0, 0, 0);

        animator.SetBool("IsJumping", pressedJump);

        if (CheckGround())
        {
            inputVelocity = Quaternion.Euler(transform.eulerAngles) * new Vector3(input.x * accel, 0f, input.y * accel);
        }
        inputVelocity += CalculateJump(inputVelocity.y);

        return inputVelocity;
    }

    private Vector3 CalculateJump(float yVelocity)
    {
        if (pressedJump && yVelocity < jumpForce && CheckGround())
        {
            Debug.Log("doing jump");
            timeSinceJump = 0;
            pressedJump = false;
            return Vector3.up * jumpForce;
        }
        else if (pressedJump && timeSinceJump > 0.1f && yVelocity < jumpForce && !CheckGround())
        {
            Debug.Log("doing vault");
            pressedJump = false;
            return CalculateVault(yVelocity);
        }

        pressedJump = false;

        return Vector3.zero;
    }

    private Vector3 CalculateVault(float yVelocity)
    {
        Debug.Log("calculating vault");
        Debug.DrawRay(vaultLoc1.position, vaultLoc1.forward * vL1Distance);
        Debug.DrawRay(vaultLoc2.position, vaultLoc2.forward * vL2Distance);
        if (!CheckGround() && !Physics.Raycast(vaultLoc1.position, vaultLoc1.forward, vL1Distance) && Physics.Raycast(vaultLoc2.position, vaultLoc2.forward, vL2Distance))
        {
            Debug.Log("vaulting");
            GetComponent<CapsuleCollider>().height = 1;
            StartCoroutine(resetHeight(0.5f));
            return vaultLoc1.forward * vaultSpeed + new Vector3(0, vaultVerticalSpeed, 0);
        }

        return Vector3.zero;
    }

    IEnumerator resetHeight(float time)
    {
        
        yield return new WaitForSeconds(time);
        GetComponent<CapsuleCollider>().height = 2;
    }

    private bool CheckGround()
    {
        bool onGround = Physics.Raycast(groundCheck.position, -groundCheck.up, 0.1f);
        animator.SetBool("IsGrounded", onGround);
        return onGround;
    }
}
