using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    public string pseudo;
    public int score;
    public int rank; // Position in the leaderboard
    public string scoreRank; // Rank name (e.g., "S", "A", "B", etc.)

    public LeaderboardEntry(string pseudo, int score)
    {
        this.pseudo = pseudo;
        this.score = score;
        this.scoreRank = GetScoreRank(score);
    }

    public string GetScoreRank(int score)
    {
        //500, 1500, 3000, 6000, 10000
        if (score >= 15000) return "SS";
        if (score >= 10000) return "S";
        if (score >= 6000) return "A";
        if (score >= 3000) return "B";
        if (score >= 1500) return "C";
        if (score >=500) return "D";
        return "Unranked";
    }
}