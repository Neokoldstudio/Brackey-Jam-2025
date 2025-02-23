using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text rankText;
    public TMP_Text actionStackText;
    public TMP_Text scoreText;
    public TMP_Text multiplierText;
    public Image rankProgressBar; // Progress bar UI element
    public Gradient rankGradient; // Color gradient for rank text

    private float displayScore;
    private float displayMultiplier;
    private List<string> recentActions = new List<string>();
    private Dictionary<string, Coroutine> actionCoroutines = new Dictionary<string, Coroutine>();
    private const int maxActions = 5;
    private const float actionFadeTime = 2.0f;
    private const float shakeIntensity = 5.0f;

    private string[] ranks = { "DULL", "MESSY", "COOL", "STYLISH", "INSANE", "ULTRA" };
    private float[] thresholds = { 500, 1500, 3000, 6000, 10000, 15000 };

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        float currentScore = ScoreManager.Instance.GetScore();
        displayScore = Mathf.Lerp(displayScore, currentScore, Time.deltaTime * 5);
        displayMultiplier = Mathf.Lerp(displayMultiplier, ScoreManager.Instance.GetMultiplier(), Time.deltaTime * 5);

        scoreText.text = Mathf.RoundToInt(displayScore).ToString();
        multiplierText.text = "x" + displayMultiplier.ToString("0.0");

        UpdateRank(currentScore);
    }

    private void UpdateRank(float score)
    {
        int rankIndex = ScoreManager.Instance.GetRank();
        rankText.text = ranks[rankIndex];
        rankText.color = rankGradient.Evaluate(rankIndex / (ranks.Length - 1));

        float progress = Mathf.Clamp01((score - thresholds[rankIndex]) / (thresholds[rankIndex+1] - thresholds[rankIndex]));
        rankProgressBar.fillAmount = Mathf.Clamp01(progress);

        float shakeAmount = Mathf.Clamp(score / 15000f, 0, 1) * shakeIntensity;
        StartCoroutine(ShakeText(rankText, shakeAmount));
    }

    private IEnumerator ShakeText(TMP_Text text, float intensity)
    {
        Vector3 originalPos = text.transform.localPosition;
        for (int i = 0; i < 10; i++)
        {
            text.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * intensity;
            yield return new WaitForSeconds(0.02f);
        }
        text.transform.localPosition = originalPos;
    }

    public void AddAction(string action)
    {
        if (recentActions.Count >= maxActions)
        {
            recentActions.RemoveAt(0);
        }
        recentActions.Add(action);
        UpdateActionStack();

        if (actionCoroutines.ContainsKey(action))
        {
            StopCoroutine(actionCoroutines[action]);
        }
        Coroutine fadeCoroutine = StartCoroutine(RemoveActionAfterTime(action, actionFadeTime));
        actionCoroutines[action] = fadeCoroutine;
    }

    private IEnumerator RemoveActionAfterTime(string action, float delay)
    {
        yield return new WaitForSeconds(delay);
        recentActions.Remove(action);
        UpdateActionStack();
        actionCoroutines.Remove(action);
    }

    private void UpdateActionStack()
    {
        actionStackText.text = string.Join("\n", recentActions);
    }
}
