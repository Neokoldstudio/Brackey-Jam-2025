using UnityEngine;

public class WaterSystem : MonoBehaviour
{
    public static WaterSystem Instance;

    [Header("Water Scaling Settings")]
    public Transform waterObject; // Assign your water cube here
    public float maxWaterHeight = 100f;
    public float baseWaterLevel = 66f;
    public float baseGrowthRate = 0.1f;
    public float growthPerHole = 0.1f;
    private int activeHoles = 0;
    private float waterLevel = 0f;

    public Transform waterPlane;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        waterLevel = baseWaterLevel;
    }

    private void Update()
    {
        if (waterObject)
        {
            if (waterLevel > maxWaterHeight)
            {
                GameManager.Instance.LoseLevel();
                return;
            }

            waterLevel += (baseGrowthRate + activeHoles * growthPerHole) * Time.deltaTime;
            Vector3 scale = waterObject.localScale;
            scale.y = Mathf.Lerp(scale.y, waterLevel, Time.deltaTime);
            waterObject.localScale = scale;
        }
    }

    public void DrainWater(float drainedAmount)
    {
        waterLevel = Mathf.Max(baseWaterLevel, waterLevel - drainedAmount);
    }

    public void RegisterHole()
    {
        activeHoles++;
    }

    public void UnregisterHole()
    {
        activeHoles = Mathf.Max(0, activeHoles - 1);
    }

    public float getWaterPlaneHeight() => waterPlane.position.y;
}
