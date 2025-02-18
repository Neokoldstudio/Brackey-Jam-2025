using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float score;
    private float styleMultiplier = 1.0f;
    private float comboDecayTimer = 3.0f;
    private float lastActionTime;

    private float rankDecayRate = 5f; // Points lost per second when inactive

    private Dictionary<string, float> actionValues = new Dictionary<string, float>()
    {
        {"Hole Plucked", 200},
        {"Dolphin Jump", 300},
        {"Ground Bounce", 50},
        {"wall Bounce", 100},
        {"Kill", 100},
        {"AerialKill", 250},
        {"Hole Plucked With Ennemy !!", 400}
    };

    private List<string> recentActions = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        float timeSinceLastAction = Time.time - lastActionTime;

        // Decay rank if no recent action
        if (timeSinceLastAction > comboDecayTimer)
        {
            score = Mathf.Max(0, score - rankDecayRate * Time.deltaTime);
            UIManager.Instance.UpdateRank(score);
        }
    }

    public void RegisterAction(string action)
    {
        if (actionValues.ContainsKey(action))
        {
            float basePoints = actionValues[action];
            float bonus = recentActions.Contains(action) ? 0.5f : 1.0f;

            score += basePoints * styleMultiplier * bonus;
            styleMultiplier += 0.1f;
            lastActionTime = Time.time;

            recentActions.Add(action);
            if (recentActions.Count > 5) recentActions.RemoveAt(0);

            UIManager.Instance.UpdateRank(score);
            UIManager.Instance.AddAction(action); // Add to UI stack
        }
    }

    public float GetScore() => score;
    public float GetMultiplier() => styleMultiplier;
}