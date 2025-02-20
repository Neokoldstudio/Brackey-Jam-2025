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

    void Awake()
    {
        Instance = this;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.m_Lens.FieldOfView = 75;
    }

    public void Shake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        multiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
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