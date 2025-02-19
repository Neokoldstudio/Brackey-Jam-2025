using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float score;
    private float styleMultiplier = 1.0f;
    public float minMultiplier = 0.5f;
    public float maxMultiplier = 5.0f;
    public float comboDecayTimer = 3.0f;
    public float multiplierDecayTime = 1.5f;
    private float lastActionTime;
    private string previousAction = "";

    private int currentRank = 0;

    public string[] ranks = { "DULL", "MESSY", "SAVAGE", "INSANE", "ULTRA" };
    public float[] thresholds = { 500, 1500, 3000, 6000, 10000 };
    public float[] rankDecayRate = { 5, 10, 15, 20, 25 }; // Points lost per second when inactive
    public float[] multiplierDecayRate = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

    private Dictionary<string, (float basePoints, float bonusMultiplier)> actionValues = new Dictionary<string, (float, float)>()
    {
        {"Hole Plucked", (200, 1.0f)},
        {"Dolphin Jump", (300, 0.2f)},
        {"Bullet Bounce", (50, 0.1f)},
        {"Wall Bounce", (100, 0.3f)},
        {"Kill", (100, 0.3f)},
        {"Boom !!", (200, 0.3f)},
        {"Airborne Kill", (250, 1.0f)},
        {"Hole Plucked With Enemy !!", (400, 2.0f)},
        {"Bullet Defflected", (20, 0.1f)},
        {"Ennemy Glued !", (10,0.1f)}
    };

    private Dictionary<string, float> currentActionValues;

    private List<string> recentActions = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        currentActionValues = new Dictionary<string, float>();
        foreach (var action in actionValues)
        {
            currentActionValues[action.Key] = action.Value.basePoints;
        }
    }

    private void Update()
    {
        float timeSinceLastAction = Time.time - lastActionTime;

        // Decay rank if no recent action
        if (timeSinceLastAction > comboDecayTimer)
        {
            score = Mathf.Max(0, score - rankDecayRate[currentRank] * Time.deltaTime);
            UpdateRank();
        }

        if (timeSinceLastAction > multiplierDecayTime)
        {
            styleMultiplier = Mathf.Max(minMultiplier, styleMultiplier - multiplierDecayRate[currentRank] * Time.deltaTime);
        }

        UIManager.Instance.UpdateRank(ranks[currentRank]);
    }

    public void RegisterAction(string action)
    {
        if (actionValues.ContainsKey(action))
        {
            if (action == previousAction)
            {
                currentActionValues[action] *= 0.5f;
            }
            else
            {
                currentActionValues[action] = actionValues[action].basePoints;
            }

            previousAction = action;

            var (basePoints, Multiplier) = (currentActionValues[action], actionValues[action].bonusMultiplier);

            score += basePoints * styleMultiplier;
            styleMultiplier += Multiplier;
            styleMultiplier = Mathf.Clamp(styleMultiplier, minMultiplier, maxMultiplier);
            lastActionTime = Time.time;
            Debug.Log(basePoints);
            recentActions.Add(action);
            if (recentActions.Count > 5) recentActions.RemoveAt(0);

            UpdateRank();
            UIManager.Instance.AddAction(action); // Add to UI stack
        }
    }

    private void UpdateRank()
    {
        int newRank = 0;
        for (int i = thresholds.Length - 1; i >= 0; i--)
        {
            if (score >= thresholds[i])
            {
                newRank = i;
                break;
            }
        }

        if (newRank != currentRank)
        {
            currentRank = newRank;
        }
    }

    public float GetScore() => score;
    public float GetMultiplier() => styleMultiplier;
    public int GetRank() => currentRank;
}
