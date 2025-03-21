using System.Collections;
using KinematicCharacterController;
using UnityEngine;

public class GunBob : MonoBehaviour
{
    public float bobSpeed = 5f;
    public float bobAmountY = 0.05f;
    public float bobAmountX = 0.02f;
    public float swayMultiplier = 0.5f;

    public float jumpOffset = 0.1f;
    public float fallOffset = 0.15f;
    public float tiltAmount = 5f;
    public float resetSpeed = 5f;

    public Transform gunTransform;
    private KinematicCharacterMotor player;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float bobTimer = 0f;

    [Header("Recoil Settings")]
    public AnimationCurve recoilCurve;
    public float recoilDuration = 0.15f;
    public float recoilKickback = 0.05f; // Reduced for a softer effect
    public float recoilRotation = 5f; // Less rotation for a more natural effect

    private bool isRecoiling;
    private Coroutine recoilCoroutine;

    private void Start()
    {
        if (gunTransform == null)
            gunTransform = transform;
        if (player == null)
            player = FindObjectOfType<KinematicCharacterMotor>();

        initialPosition = gunTransform.localPosition;
        initialRotation = gunTransform.localRotation;
    }

    private void Update()
    {
        bool isGrounded = GameManager.Instance.isPlayerOnGround();
        bool isUnderwater = GameManager.Instance.isPlayerUnderwater();

        Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float verticalVelocity = player.Velocity.y;

        if (velocity.magnitude > 0.1f && isGrounded)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmountY;
            float bobOffsetX = Mathf.Cos(bobTimer * swayMultiplier) * bobAmountX;
            gunTransform.localPosition = initialPosition + new Vector3(bobOffsetX, bobOffsetY, 0);
        }
        else
        {
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, initialPosition, Time.deltaTime * resetSpeed);
            bobTimer = 0;
        }

        Vector3 jumpFallOffset = Vector3.zero;
        Quaternion jumpFallRotation = Quaternion.identity;

        if (verticalVelocity > 0.1f && !isUnderwater)
        {
            jumpFallOffset = new Vector3(0, jumpOffset * Mathf.Abs(verticalVelocity), 0);
            jumpFallRotation = Quaternion.Euler(-tiltAmount * verticalVelocity, 0, 0);
        }
        else if (verticalVelocity < -0.1f && !isUnderwater)
        {
            jumpFallOffset = new Vector3(0, -fallOffset * Mathf.Abs(verticalVelocity), 0);
            jumpFallRotation = Quaternion.Euler(tiltAmount * Mathf.Abs(verticalVelocity), 0, 0);
        }

        gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, initialPosition + jumpFallOffset, Time.deltaTime * resetSpeed);
        gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, initialRotation * jumpFallRotation, Time.deltaTime * resetSpeed);
    }

    public void TriggerRecoil()
    {
        if (isRecoiling) return; // Prevent double animation
        if (recoilCoroutine != null) StopCoroutine(recoilCoroutine);
        recoilCoroutine = StartCoroutine(RecoilCoroutine());
    }

    private IEnumerator RecoilCoroutine()
    {
        isRecoiling = true;

        Vector3 recoilEndPos = initialPosition + Vector3.back * recoilKickback;
        Quaternion recoilEndRot = initialRotation * Quaternion.Euler(-recoilRotation, 0, 0);

        float elapsed = 0f;

        // Smooth recoil kick
        while (elapsed < recoilDuration * 0.4f)
        {
            float t = elapsed / (recoilDuration * 0.4f);
            float curveValue = recoilCurve.Evaluate(t);

            gunTransform.localPosition = Vector3.Lerp(initialPosition, recoilEndPos, curveValue);
            gunTransform.localRotation = Quaternion.Slerp(initialRotation, recoilEndRot, curveValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hold position briefly before returning
        yield return new WaitForSeconds(0.05f);

        // Smooth return to normal
        elapsed = 0f;
        while (elapsed < recoilDuration * 0.6f)
        {
            float t = elapsed / (recoilDuration * 0.6f);
            float curveValue = 1f - recoilCurve.Evaluate(t); // Reverse curve for return

            gunTransform.localPosition = Vector3.Lerp(recoilEndPos, initialPosition, curveValue);
            gunTransform.localRotation = Quaternion.Slerp(recoilEndRot, initialRotation, curveValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure it's exactly back
        gunTransform.localPosition = initialPosition;
        gunTransform.localRotation = initialRotation;

        isRecoiling = false;
    }
}
