using UnityEngine;

public class WaterSystem : MonoBehaviour
{
    public static WaterSystem Instance;

    [Header("Water Scaling Settings")]
    public Transform waterObject; // Assign your water cube here
    public float maxWaterHeight = 10f;
    public float baseGrowthRate = 0.1f;
    public float growthPerHole = 0.1f;
    private int activeHoles = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (waterObject)
        {
            Vector3 scale = waterObject.localScale;
            scale.y += (baseGrowthRate + activeHoles * growthPerHole) * Time.deltaTime;
            waterObject.localScale = scale;
        }
    }

    public void RegisterHole()
    {
        activeHoles++;
    }

    public void UnregisterHole()
    {
        activeHoles = Mathf.Max(0, activeHoles - 1);
    }
}