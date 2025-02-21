using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankDisplayScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int rank = ScoreManager.Instance.GetRank();
        this.transform.GetChild(rank).gameObject.SetActive(true);
    }
}
