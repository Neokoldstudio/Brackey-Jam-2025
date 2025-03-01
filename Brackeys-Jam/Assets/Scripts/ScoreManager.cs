using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float score;
    private float styleMultiplier = 1.0f;
    public float minMultiplier = 1.0f;
    public float maxMultiplier = 5.0f;
    public float multiplierDecayTime = 1.5f;
    public float airTimeMultiplier = 1f;

    private float lastActionTime;
    private float lastMultiplierTime;
    private float currentAirTime = 0.0f;
    private string previousAction = "";
    private bool canUpdate = true;

    private int currentRank = 0;

    private string[] ranks = { "DULL", "MESSY", "COOL", "STYLISH", "INSANE", "ULTRA" };
    private float[] thresholds = { 500, 1500, 3000, 6000, 10000, 15000 };
    public float[] ScoreDecayTimer = {};
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
        {"Ennemy Glued !", (10,0.1f)},
        {"Airborne Plucking!", (300, 1.5f)},
        {"Hurt", (-200, -5.0f)}
    };

    private Dictionary<string, float> currentActionValues;

    private List<string> recentActions = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        DontDestroyOnLoad(gameObject);
        currentActionValues = new Dictionary<string, float>();
        foreach (var action in actionValues)
        {
            currentActionValues[action.Key] = action.Value.basePoints;
        }
    }

    private void Update()
    {
        if (canUpdate)
        {
            //air time effecting multiplier logic
            if (!GameManager.Instance.isPlayerOnGround() && !GameManager.Instance.isPlayerUnderwater())
            {
                if(currentAirTime> 2.0)styleMultiplier += Time.deltaTime/3;
                currentAirTime += Time.deltaTime;
            }
            else
            {
                score += currentAirTime * airTimeMultiplier * styleMultiplier;
                currentAirTime = 0.0f;
            }
            styleMultiplier = Mathf.Clamp(styleMultiplier, minMultiplier, maxMultiplier);

            float timeSinceLastAction = Time.time - lastActionTime;
            float timeSinceLastMult = Time.time - lastMultiplierTime;

            // Decay rank if no recent action
            if (timeSinceLastAction > ScoreDecayTimer[currentRank])
            {
                score = Mathf.Max(0, score - rankDecayRate[currentRank] * Time.deltaTime);
                UpdateRank();
            }

            if (timeSinceLastMult > multiplierDecayTime)
            {
                styleMultiplier = Mathf.Max(minMultiplier, styleMultiplier - multiplierDecayRate[currentRank] * Time.deltaTime);
            }
        }
    }

    public void RegisterAction(string action)
    {
        if (actionValues.ContainsKey(action))
        {
            var (basePoints, Multiplier) = (currentActionValues[action], actionValues[action].bonusMultiplier);

            if (action == previousAction && action!="Hurt")
            {
                currentActionValues[action] *= 0.5f;
            }
            else
            {
                currentActionValues[action] = actionValues[action].basePoints;
                lastMultiplierTime = Time.time;
                styleMultiplier += Multiplier;
            }

            previousAction = action;

            styleMultiplier = Mathf.Clamp(styleMultiplier, minMultiplier, maxMultiplier);

            score += basePoints * styleMultiplier;

            if (score <= 0) score = 0;

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

    public void FreezeScore()
    {
        canUpdate = false;
    }

    public void UnfreezeScore()
    {
        canUpdate = true;
    }

    public void Reset()
    {
        currentRank = 0;
        score = 0;
        styleMultiplier = 1.0f;
        lastActionTime = 0f;
        previousAction = "";
        UnfreezeScore();
    }

    public float GetScore() => score;
    public float GetMultiplier() => styleMultiplier;
    public int GetRank() => currentRank;
    public float GetAirTime() => currentAirTime;
    public float GetMaxMultiplier() => maxMultiplier;
}
