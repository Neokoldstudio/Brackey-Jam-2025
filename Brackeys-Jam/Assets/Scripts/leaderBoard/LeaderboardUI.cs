using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject entryPrefab;  // Prefab for leaderboard row
    public Transform contentParent; // Parent of the scrollable list
    public ScrollRect scrollRect;

    private void Start()
    {
        SetupContentLayout(); // Ensure proper layout setup
        DisplayLeaderboard();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void SetupContentLayout()
    {
        VerticalLayoutGroup layoutGroup = contentParent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = contentParent.gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 10;
        }

        ContentSizeFitter sizeFitter = contentParent.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = contentParent.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    public void DisplayLeaderboard()
    {
        // Clear previous entries
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        List<LeaderboardEntry> entries = LeaderboardManager.Instance.entries;
        foreach (LeaderboardEntry entry in entries)
        {
            GameObject newEntry = Instantiate(entryPrefab, contentParent);
            TMP_Text[] texts = newEntry.GetComponentsInChildren<TMP_Text>();

            texts[0].text = "#" + entry.rank;
            texts[1].text = entry.pseudo;
            texts[2].text = entry.score.ToString();
            texts[3].text = entry.scoreRank;
        }
    }
}
