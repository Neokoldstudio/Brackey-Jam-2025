using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text rankText;
    public TMP_Text actionStackText;
    public TMP_Text scoreText;
    public TMP_Text multiplierText;
    public TMP_Text airTimeText;
    public GameObject progressBarContainer;
    public Image rankProgressBar;
    public Gradient multiplierGradient; // Gradient for multiplier text

    private float displayScore;
    private float displayMultiplier;
    private List<string> recentActions = new List<string>();
    private Dictionary<string, Coroutine> actionCoroutines = new Dictionary<string, Coroutine>();
    private const int maxActions = 5;
    private const float actionFadeTime = 2.0f;

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
        displayMultiplier = Mathf.Lerp(displayMultiplier, ScoreManager.Instance.GetMultiplier(), Time.deltaTime * 10);

        scoreText.text = Mathf.RoundToInt(displayScore).ToString();
        multiplierText.text = displayMultiplier>1.01 ? "x" + displayMultiplier.ToString("0.00") : "";

        // Apply gradient color to the multiplier based on its value
        float maxMultiplier = ScoreManager.Instance.GetMaxMultiplier();
        float normalizedMultiplier = Mathf.Clamp01((displayMultiplier-1)/(maxMultiplier-1));
        multiplierText.color = multiplierGradient.Evaluate(normalizedMultiplier);

        if (ScoreManager.Instance.GetAirTime() <= 1.0f)
        {
            airTimeText.text = "";
        }
        else
        {
            airTimeText.text = "Air Time: " + ScoreManager.Instance.GetAirTime().ToString("0.0");
        }

        UpdateRank(currentScore);
    }

    private void UpdateRank(float score)
    {
        if (score < thresholds[0])
        {
            progressBarContainer.SetActive(false);
        }
        else
        {
            progressBarContainer.SetActive(true);
        }

        int rankIndex = ScoreManager.Instance.GetRank();
        rankText.text = score > thresholds[rankIndex] ? ranks[rankIndex] : "";
        float progress = (rankIndex < thresholds.Length - 1) ?
            Mathf.Clamp01((score - thresholds[rankIndex]) / (thresholds[rankIndex + 1] - thresholds[rankIndex])) :
            Mathf.Clamp01((score - thresholds[rankIndex]) / (20000 - thresholds[rankIndex]));

        rankProgressBar.fillAmount = Mathf.Clamp01(progress);
    }

    public void AddAction(string action)
    {
        if (recentActions.Count >= maxActions)
        {
            recentActions.RemoveAt(0);
        }
        recentActions.Add(action);
        UpdateActionStack();

        Coroutine fadeCoroutine = StartCoroutine(RemoveActionAfterTime(action, actionFadeTime));
        actionCoroutines[action] = fadeCoroutine;
    }

    public void SetMultiplierText()
    {
        displayMultiplier = ScoreManager.Instance.GetMultiplier();
        Debug.Log(displayMultiplier);
        multiplierText.text = displayMultiplier > 1.01 ? "x" + displayMultiplier.ToString("0.00") : "";
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
