using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkGamePlayerIsland : NetworkBehaviour
{
    [SyncVar] public string displayName;

    [Scene] [SerializeField] private string lobbyScene;

    [Header("First Person")]
    [SerializeField] public float sens = 10F;
    [SerializeField] private Transform playerCamera;

    [Header("Movement")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float acceleration;
    [SerializeField] private float friction;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCrouchCheck;

    [Header("Vaulting")]
    [SerializeField] private Transform vaultLoc1;
    [SerializeField] private Transform vaultLoc2;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private float vL1Distance;
    [SerializeField] private float vL2Distance;
    [SerializeField] private float vaultSpeed;
    [SerializeField] private float vaultVerticalSpeed;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    // Rotation values
    private float lookMinY = -90F;
    private float lookMaxX = 90F;

    float lookYRotation = 0F;

    private bool jumpKeyPressed = false;
    private bool isCrouching = false;
    private float timeSinceCrouch = 0;
    private float timeSinceJump = 0;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void OnEnable()
    {
        NetworkManagerIsland.OnClientDisconnected += ReturnToMainMenu;
    }

    void OnDisable()
    {
        NetworkManagerIsland.OnClientDisconnected -= ReturnToMainMenu;
    }

    void Update()
    {

        DoButtons();
        DoLook();
        CmdUpdateAnimations(rb.velocity.magnitude > 0.5, jumpKeyPressed, CheckGround(), isCrouching);
    }

    [Command]
    private void CmdUpdateAnimations(bool isRunning, bool isJumping, bool isGrounded, bool isCrouching)
    {
        RpcUpdateAnimations(isRunning, isJumping, isGrounded, isCrouching);
    }

    void FixedUpdate()
    {
        timeSinceJump += Time.fixedDeltaTime;
        timeSinceCrouch += Time.fixedDeltaTime;

        if (hasAuthority)
        {
            Vector3 playerVelocity = rb.velocity;

            playerVelocity = CalculateFriction(playerVelocity);
            playerVelocity += CalculateMovement(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), playerVelocity);

            rb.velocity = playerVelocity;
        }
    }

    private void DoButtons()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpKeyPressed = true;
            }

            // Toggle cursor
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
            }

            if (!isCrouching && Input.GetKey(KeyCode.LeftControl))
            {
                transform.position = transform.position - new Vector3(0, 0.5f, 0);
            }
            else if (isCrouching && !Input.GetKey(KeyCode.LeftControl))
            {
                transform.position = transform.position + new Vector3(0, 0.5f, 0);
            }
            isCrouching = Input.GetKey(KeyCode.LeftControl);
        }
    }

    private void DoLook()
    {
        playerCamera.gameObject.SetActive(hasAuthority);
        if (hasAuthority)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sens;

            lookYRotation += Input.GetAxis("Mouse Y") * sens;
            lookYRotation = Mathf.Clamp(lookYRotation, lookMinY, lookMaxX);

            playerCamera.transform.localEulerAngles = new Vector3(-lookYRotation, 0, 0);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
        }
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene(lobbyScene);
    }

    // =====
    // Movement
    // =====

    private Vector3 CalculateFriction(Vector3 currentVelocity)
    {
        float speed = currentVelocity.magnitude;

        if (!CheckGround() || Input.GetKeyDown(KeyCode.Space) || speed == 0) return currentVelocity * (1 - (0.1f * Time.fixedDeltaTime));

        if (isCrouching)
        {
            float crouchDrop = speed * 8 * Time.deltaTime;
            return currentVelocity * (Mathf.Max(speed - crouchDrop, 0f) / speed);
        }

        float drop = speed * friction * Time.deltaTime;
        return currentVelocity * (Mathf.Max(speed - drop, 0f) / speed);
    }

    private Vector3 CalculateMovement(Vector2 input, Vector3 currentVelocity)
    {
        Vector3 inputVelocity = new Vector3(0, 0, 0);

        if (CheckGround())
        {
            inputVelocity = Quaternion.Euler(transform.eulerAngles) * new Vector3(input.x * acceleration, 0f, input.y * acceleration);
        }

        if (isCrouching)
        {
            inputVelocity /= 2;
        }

        inputVelocity += CalculateCrouching(currentVelocity);

        if (!isCrouching)
        {
            inputVelocity += CalculateJump(inputVelocity.y);
        }

        return inputVelocity;
    }

    private Vector3 CalculateCrouching(Vector3 currentVelocity)
    {
        if (isCrouching && capsuleCollider.height == 2 && timeSinceCrouch > 1)
        {
            timeSinceCrouch = 0;

            if (CheckGround())
                return currentVelocity * 1.2f;
        }

        capsuleCollider.height = isCrouching ? 1 : 2;
        return Vector3.zero;
    }

    private Vector3 CalculateJump(float yVelocity)
    {
        if (jumpKeyPressed && yVelocity < jumpForce && CheckGround())
        {
            timeSinceJump = 0;
            jumpKeyPressed = false;
            return Vector3.up * jumpForce;
        }
        else if (jumpKeyPressed && timeSinceJump > 0.1f && yVelocity < jumpForce && !CheckGround())
        {
            jumpKeyPressed = false;
            return CalculateVault(yVelocity);
        }

        jumpKeyPressed = false;

        return Vector3.zero;
    }

    private Vector3 CalculateVault(float yVelocity)
    {
        // Debug.DrawRay(vaultLoc1.position, vaultLoc1.forward * vL1Distance);
        // Debug.DrawRay(vaultLoc2.position, vaultLoc2.forward * vL2Distance);
        if (!CheckGround() && !Physics.Raycast(vaultLoc1.position, vaultLoc1.forward, vL1Distance) && Physics.Raycast(vaultLoc2.position, vaultLoc2.forward, vL2Distance))
        {
            capsuleCollider.height = 1;
            StartCoroutine(resetHeight(0.5f));
            return vaultLoc1.forward * vaultSpeed + new Vector3(0, vaultVerticalSpeed, 0);
        }

        return Vector3.zero;
    }

    IEnumerator resetHeight(float time)
    {
        yield return new WaitForSeconds(time);
        capsuleCollider.height = 2;
    }

    private bool CheckGround()
    {
        bool onGround;
        if (isCrouching)
        {
            onGround = Physics.Raycast(groundCrouchCheck.position, -groundCheck.up, 0.1f);
        }
        else
        {
            onGround = Physics.Raycast(groundCheck.position, -groundCheck.up, 0.1f);
        }
        return onGround;
    }

    // =====
    // Animations
    // =====

    [ClientRpc]
    private void RpcUpdateAnimations(bool isRunning, bool isJumping, bool isGrounded, bool isCrouching)
    {
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
    }
}
