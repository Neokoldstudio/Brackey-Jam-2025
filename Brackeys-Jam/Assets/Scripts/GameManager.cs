using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float levelDuration = 60f;
    private float levelTimer;
    public int playerScore = 0;

    [Header("Hole Spawning")]
    public List<HoleSpawner> holeSpawners = new List<HoleSpawner>();
    private int maxActiveHoles;
    public float spawnInterval = 5f;
    private float spawnTimer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        levelTimer = levelDuration;
        spawnTimer = spawnInterval;
        FindHoleSpawners();
        maxActiveHoles = holeSpawners.Count;
    }

    private void Update()
    {
        UpdateLevelTimer();
        HandleHoleSpawning();
    }

    private void UpdateLevelTimer()
    {
        if (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
            if (levelTimer <= 0)
            {
                EndLevel();
            }
        }
    }

    private void HandleHoleSpawning()
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
            return;
        }

        spawnTimer = spawnInterval;

        if (CountActiveHoles() >= maxActiveHoles)
            return;

        int randomIndex = Random.Range(0, holeSpawners.Count);
        HoleSpawner spawner = holeSpawners[randomIndex];
        while (spawner.SpawnHole() == false)
        {
            randomIndex = Random.Range(0, holeSpawners.Count);
            spawner = holeSpawners[randomIndex];
        }
    }

    private void FindHoleSpawners()
    {
        HoleSpawner[] foundSpawners = FindObjectsOfType<HoleSpawner>();
        holeSpawners = new List<HoleSpawner>(foundSpawners);
    }

    private int CountActiveHoles()
    {
        int count = 0;
        foreach (HoleSpawner spawner in holeSpawners)
        {
            if (spawner.HasHole()) count++;
        }
        return count;
    }

    public void IncreaseScore(int amount)
    {
        playerScore += amount;
    }

    private void EndLevel()
    {
        Debug.Log("Level Complete! Score: " + playerScore);
        // Implement level transition logic here
    }
}
