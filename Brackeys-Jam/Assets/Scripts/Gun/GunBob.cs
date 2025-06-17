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
    private bool RecoilActive = false;

    [Header("Recoil Settings")]
    public float recoilKickback = 0.05f;
    public float recoilRotation = 5f;
    public float recoilResetSpeed = 0.15f;

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

        if (velocity.magnitude > 0.1f && isGrounded && !RecoilActive)
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
        float recoilZ = -recoilKickback * Random.Range(0.1f, 1.2f);
        float recoilXRotation = Random.Range(recoilRotation*0.1f, recoilRotation*1.2f);
        float recoilZRotation = Random.Range(recoilRotation * 1f, recoilRotation * -1f);

        gunTransform.localPosition += new Vector3(0, 0, recoilZ);
        gunTransform.localRotation *= Quaternion.Euler(0, 0, recoilZRotation);
        gunTransform.localRotation *= Quaternion.Euler(-recoilXRotation, 0, 0);

        RecoilActive = true;
        StartCoroutine(RecoilCoroutine());
    }

    private IEnumerator RecoilCoroutine()
    {
        float elapsed = 0f;
        while (elapsed <= recoilResetSpeed)
        {
            Debug.Log("Recoil active, resetting position and rotation.");
            elapsed += Time.deltaTime;
            gunTransform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * recoilResetSpeed);
            gunTransform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation, Time.deltaTime * recoilResetSpeed);
            yield return null;
        }

        gunTransform.localPosition = initialPosition;
        gunTransform.localRotation = initialRotation;
        RecoilActive = false;
    }
}
