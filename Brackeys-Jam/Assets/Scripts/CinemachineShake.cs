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
    public float vertigoAmount = 1f;
    public float speedThreshold = 15f;
    public float shakeSpeed = 10f; //controls the perlin noise scrolling speed

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
        float randomStartX = Random.Range(-1000f, 1000f);
        float randomStartY = Random.Range(-1000f, 1000f);

        while (elapsed < duration)
        {
            float x = Mathf.PerlinNoise(randomStartX + (elapsed/duration)*shakeSpeed, randomStartY)*2-1;
            float y = Mathf.PerlinNoise(randomStartX, randomStartY + (elapsed / duration) * shakeSpeed) *2-1;

            cameraFollowTarget.localPosition = originalPosition + new Vector3(x, y, 0) * intensity;
            virtualCamera.m_Lens.Dutch = Mathf.Lerp(virtualCamera.m_Lens.Dutch, x, 0.1f);

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

        if(playerVel.magnitude>speedThreshold) virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, 75 + (playerVel.magnitude-speedThreshold) * vertigoAmount,0.1f);
        else virtualCamera.m_Lens.FieldOfView = 75;
    }
}