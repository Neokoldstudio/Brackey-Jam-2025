using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RankDisplayScene : MonoBehaviour
{

    public TMP_Text scoreText;
    public TMP_InputField pseudoInput;
    public LeaderboardManager leaderboard;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        int rank = ScoreManager.Instance.GetRank();
        float score = Mathf.RoundToInt(ScoreManager.Instance.GetScore());
        this.transform.GetChild(rank).gameObject.SetActive(true);
        scoreText.text = score.ToString();
    }

    public void ChangeScene(int scene)
    {
        leaderboard.AddEntry(pseudoInput.text, Mathf.RoundToInt(ScoreManager.Instance.GetScore()));
        ScoreManager.Instance.Reset();
        SceneManager.LoadScene(scene);
    }
}
