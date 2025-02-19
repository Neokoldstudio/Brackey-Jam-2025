using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text rankText; // Displays current rank
    public TMP_Text actionStackText; // Displays recent actions
    public TMP_Text scoreText;
    public TMP_Text multiplierText;

    private float displayScore;
    private float displayMultiplier;
    private List<string> recentActions = new List<string>();
    private const int maxActions = 5;

    private string[] ranks = { "DULL", "MESSY", "SAVAGE", "INSANE", "ULTRA" };
    private float[] thresholds = { 500, 1500, 3000, 6000, 10000 };

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        // Smooth rank transition
        displayScore = Mathf.Lerp(displayScore, ScoreManager.Instance.GetScore(), Time.deltaTime * 5);
        displayMultiplier = Mathf.Lerp(displayMultiplier, ScoreManager.Instance.GetMultiplier(), Time.deltaTime * 5);
        scoreText.text = Mathf.RoundToInt(displayScore).ToString();
        multiplierText.text = "x" + displayMultiplier.ToString("0.0");
    }

    public void UpdateRank(string rank)
    {
        rankText.text = rank;
    }

    public void AddAction(string action)
    {
        if (recentActions.Count >= maxActions)
        {
            recentActions.RemoveAt(0); // Remove oldest action
        }
        recentActions.Add(action);
        UpdateActionStack();
    }

    private void UpdateActionStack()
    {
        actionStackText.text = string.Join("\n", recentActions);
    }
}
