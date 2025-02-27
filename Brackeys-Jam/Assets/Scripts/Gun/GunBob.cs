using KinematicCharacterController;
using UnityEngine;

public class GunBob : MonoBehaviour
{
    public float bobSpeed = 5f;        // Speed of bobbing
    public float bobAmountY = 0.05f;   // Up/down bobbing amount
    public float bobAmountX = 0.02f;   // Side-to-side sway amount
    public float swayMultiplier = 0.5f; // Adjusts sideways movement

    public float jumpOffset = 0.1f;    // How much the gun moves up when jumping
    public float fallOffset = 0.15f;   // How much the gun moves down when falling
    public float tiltAmount = 5f;      // How much the gun tilts when jumping/falling
    public float resetSpeed = 5f;      // How quickly the gun resets position/rotation

    public Transform gunTransform; // Assign the gun's transform in Inspector
    private KinematicCharacterMotor player; // Assign the player's CharacterController

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float bobTimer = 0f;

    private void Start()
    {
        if (gunTransform == null)
            gunTransform = transform; // Default to self if not set
        if (player == null)
            player = FindObjectOfType<KinematicCharacterMotor>(); // Auto-find player

        initialPosition = gunTransform.localPosition;
        initialRotation = gunTransform.localRotation;
    }

    private void Update()
    {
        bool isGrounded = GameManager.Instance.isPlayerOnGround();
        bool isUnderwater = GameManager.Instance.isPlayerUnderwater();

        Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float verticalVelocity = player.Velocity.y; // Get vertical speed

        // Calculate bobbing
        if (velocity.magnitude > 0.1f && isGrounded) // Only bob when moving
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmountY;
            float bobOffsetX = Mathf.Cos(bobTimer * swayMultiplier) * bobAmountX;
            gunTransform.localPosition = initialPosition + new Vector3(bobOffsetX, bobOffsetY, 0);
        }
        else
        {
            // Smoothly reset position when stopping
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, initialPosition, Time.deltaTime * resetSpeed);
            bobTimer = 0;
        }

        // Jump/Fall effect
        Vector3 jumpFallOffset = Vector3.zero;
        Quaternion jumpFallRotation = Quaternion.identity;

        if (verticalVelocity > 0.1f && !isUnderwater) // Jumping
        {
            jumpFallOffset = new Vector3(0, jumpOffset * Mathf.Abs(verticalVelocity), 0);
            jumpFallRotation = Quaternion.Euler(-tiltAmount * verticalVelocity, 0, 0); // Tilt up
        }
        else if (verticalVelocity < -0.1f && !isUnderwater) // Falling
        {
            jumpFallOffset = new Vector3(0, -fallOffset * Mathf.Abs(verticalVelocity), 0);
            jumpFallRotation = Quaternion.Euler(tiltAmount * Mathf.Abs(verticalVelocity), 0, 0); // Tilt down
        }

        // Apply jump/fall effects smoothly
        gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, initialPosition + jumpFallOffset, Time.deltaTime * resetSpeed);
        gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, initialRotation * jumpFallRotation, Time.deltaTime * resetSpeed);
    }
}