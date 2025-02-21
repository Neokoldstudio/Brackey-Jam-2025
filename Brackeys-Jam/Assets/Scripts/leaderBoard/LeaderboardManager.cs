using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private const string FILE_NAME = "leaderboard.json";
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        LoadLeaderboard();
    }

    public void AddEntry(string pseudo, int score)
    {
        LeaderboardEntry newEntry = new LeaderboardEntry(pseudo, score);
        entries.Add(newEntry);
        SortAndRankEntries();
        SaveLeaderboard();
    }

    private void SortAndRankEntries()
    {
        entries.Sort((a, b) => b.score.CompareTo(a.score)); // Higher scores first
        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].rank = i + 1;
        }
    }

    public void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(new LeaderboardData(entries), true);
        File.WriteAllText(GetFilePath(), json);
    }

    public void LoadLeaderboard()
    {
        if (File.Exists(GetFilePath()))
        {
            string json = File.ReadAllText(GetFilePath());
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            entries = data.entries ?? new List<LeaderboardEntry>();
            SortAndRankEntries();
        }
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, FILE_NAME);
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries;
    public LeaderboardData(List<LeaderboardEntry> entries) { this.entries = entries; }
}