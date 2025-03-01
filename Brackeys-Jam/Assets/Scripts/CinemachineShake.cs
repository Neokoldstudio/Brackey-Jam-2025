using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{

    public static CinemachineShake Instance { get; private set; }
    private CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;

    public float tiltingAmount = 0.5f;
    public float vertigoAmount = 0.5f;
    public float speedThreshold = 15f;

    public Transform cameraFollowTarget; // The empty object Cinemachine follows
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        Instance = this;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.m_Lens.FieldOfView = 75;
        originalPosition = cameraFollowTarget.localPosition;
    }


    public void Shake(float intensity, float duration)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            cameraFollowTarget.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraFollowTarget.localPosition = originalPosition; // Reset after shake
    }


    private void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                multiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }

        Vector3 playerVel = GameManager.Instance.getPlayerVelocity();
        float tiltAmount = playerVel.x * -tiltingAmount;
        virtualCamera.m_Lens.Dutch = Mathf.Lerp(virtualCamera.m_Lens.Dutch, tiltAmount, 0.1f);

        if(playerVel.magnitude>speedThreshold && playerVel.y<-15) virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, 75 + (playerVel.magnitude-speedThreshold) * vertigoAmount,0.1f);
        else virtualCamera.m_Lens.FieldOfView = 75;
    }
}