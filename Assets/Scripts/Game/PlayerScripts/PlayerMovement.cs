using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] public float sens = 10F;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float lookMinY = -90F;
    [SerializeField] private float lookMaxX = 90F;

    [Header("Movement Values")]
    [SerializeField] private float acceleration;
    [SerializeField] private float sprintAcceleration;
    [SerializeField] private float friction;
    [SerializeField] private float airFriction;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityForce;

    [Header("Movement References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCrouchCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private int[] ignoreCollisions;

    [Header("Vaulting")]
    [SerializeField] private Transform vaultLoc1;
    [SerializeField] private Transform vaultLoc2;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private float vL1Distance;
    [SerializeField] private float vL2Distance;
    [SerializeField] private float vaultSpeed;
    [SerializeField] private float vaultVerticalSpeed;

    private Rigidbody rb;

    private float lookYRotation = 0;

    private float timeSinceJump = 0;
    private float timeSinceCrouch = 0;

    private Vector3 currentGravityForce = Vector3.zero;

    private bool jumpKeyPressed = false;

    public bool isJumping { get; private set; } = false;
    public bool isCrouching { get; private set; } = false;
    public bool isSprinting { get; private set; } = false;
    [SerializeField] public bool isGrounded { get; private set; } = false;

    public bool movementEnabled { get; private set; } = true;
    public bool lookEnabled { get; private set; } = true;

    // ===== Unity methods

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Ignore collisions with the layers in the ignoreCollisions variable
        for (int i = 0; i < ignoreCollisions.Length; i++)
            Physics.IgnoreLayerCollision(gameObject.layer, ignoreCollisions[i]);
    }

    void Update()
    {
        DoButtons();
        DoLook();
        CheckGround();
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
            playerVelocity += CalculateGravity();

            if (playerVelocity.normalized.magnitude > 0)
                rb.velocity = playerVelocity;
        }
    }

    // ===== Public methods

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public void ToggleMovement(bool enabled)
    {
        movementEnabled = enabled;
    }

    public void ToggleLook(bool enabled)
    {
        lookEnabled = enabled;
    }

    // ===== Private methods

    private void DoButtons()
    {
        if (hasAuthority)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                jumpKeyPressed = true;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isSprinting = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isSprinting = false;
            }

            if (!isCrouching && Input.GetKey(KeyCode.LeftControl) && movementEnabled) // Start crouching
            {
                isCrouching = true;
                transform.position = transform.position - new Vector3(0, 0.5f, 0);
            }
            else if (isCrouching && !Input.GetKey(KeyCode.LeftControl) && movementEnabled) // Stop crouching
            {
                isCrouching = false;
                transform.position = transform.position + new Vector3(0, 0.5f, 0);
            }
        }
    }

    private void DoLook()
    {
        playerCamera.gameObject.SetActive(hasAuthority);
        if (hasAuthority && lookEnabled)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sens;

            lookYRotation += Input.GetAxis("Mouse Y") * sens;
            lookYRotation = Mathf.Clamp(lookYRotation, lookMinY, lookMaxX);

            playerCamera.transform.localEulerAngles = new Vector3(-lookYRotation, 0, 0);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
        }
    }

    private Vector3 CalculateFriction(Vector3 currentVelocity)
    {
        float speed = currentVelocity.magnitude;

        if (!isGrounded || Input.GetKeyDown(KeyCode.Space) || speed == 0)
        {
            float jumpDrop = speed * airFriction * Time.deltaTime;
            return currentVelocity * (Mathf.Max(speed - jumpDrop, 0f) / speed);
        }

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
        if (movementEnabled)
        {
            if (isGrounded)
            {
                float accl = isSprinting ? sprintAcceleration : acceleration;
                // Only allow sprinting forward
                inputVelocity = Quaternion.Euler(transform.eulerAngles) * new Vector3(input.x * acceleration, 0f, input.y * (input.y > 0 ? accl : acceleration));

                // Halve movement speed while crouching
                if (isCrouching)
                    inputVelocity /= 2;
            }

            inputVelocity += CalculateCrouching(currentVelocity);

            if (!isCrouching)
            {
                inputVelocity += CalculateJump(inputVelocity.y);
            }
        }

        return inputVelocity;
    }

    private Vector3 CalculateGravity()
    {
        if (isGrounded)
        {
            currentGravityForce = Vector3.zero;
        }

        if (!isGrounded)
        {
            if (currentGravityForce.magnitude <= 0)
                currentGravityForce = new Vector3(0, -gravityForce, 0);
            else
                currentGravityForce += currentGravityForce * Time.fixedDeltaTime;
        }

        return currentGravityForce;
    }

    private Vector3 CalculateCrouching(Vector3 currentVelocity)
    {
        if (isCrouching && capsuleCollider.height == 2 && timeSinceCrouch > 1)
        {
            timeSinceCrouch = 0;

            if (isGrounded)
                return currentVelocity * 1.2f;
        }

        capsuleCollider.height = isCrouching ? 1 : 2;
        return Vector3.zero;
    }

    private Vector3 CalculateJump(float yVelocity)
    {
        if (jumpKeyPressed && yVelocity < jumpForce && isGrounded && timeSinceJump > 1f)
        {
            timeSinceJump = 0;
            jumpKeyPressed = false;
            StartCoroutine(StartJump(0.5f));
            return Vector3.up * jumpForce;
        }
        else if (jumpKeyPressed && timeSinceJump > 0.5f && yVelocity < jumpForce && !isGrounded)
        {
            jumpKeyPressed = false;
            // return CalculateVault(yVelocity);
        }

        jumpKeyPressed = false;

        return Vector3.zero;
    }

    private Vector3 CalculateVault(float yVelocity)
    {
        Debug.DrawRay(vaultLoc1.position, vaultLoc1.forward * vL1Distance);
        Debug.DrawRay(vaultLoc2.position, vaultLoc2.forward * vL2Distance);
        if (!isGrounded && !Physics.Raycast(vaultLoc1.position, vaultLoc1.forward, vL1Distance) && Physics.Raycast(vaultLoc2.position, vaultLoc2.forward, vL2Distance))
        {
            capsuleCollider.height = 1;
            StartCoroutine(ResetHeight(0.5f));
            return vaultLoc1.forward * vaultSpeed + new Vector3(0, vaultVerticalSpeed, 0);
        }

        return Vector3.zero;
    }

    private void CheckGround()
    {
        if (isCrouching)
        {
            isGrounded = Physics.CheckBox(groundCrouchCheck.position, new Vector3(0.5f, 0.1f, 0.5f), Quaternion.Euler(0, 0, 0), groundMask);
        }
        else
        {
            isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(0.15f, 0.1f, 0.15f), Quaternion.Euler(0, 0, 0), groundMask);
        }
    }

    private IEnumerator ResetHeight(float time)
    {
        yield return new WaitForSeconds(time);
        capsuleCollider.height = 2;
    }

    private IEnumerator StartJump(float duration)
    {
        isJumping = true;
        yield return new WaitForSeconds(duration);
        isJumping = false;
    }
}
