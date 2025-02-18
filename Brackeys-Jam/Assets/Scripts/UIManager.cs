using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text rankText; // Displays current rank
    public TMP_Text actionStackText; // Displays recent actions

    private float displayScore;
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
        UpdateRank(displayScore);
    }

    public void UpdateRank(float score)
    {
        string newRank = "DULL";
        for (int i = thresholds.Length - 1; i >= 0; i--)
        {
            if (score >= thresholds[i])
            {
                newRank = ranks[i];
                break;
            }
        }
        rankText.text = newRank;
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
