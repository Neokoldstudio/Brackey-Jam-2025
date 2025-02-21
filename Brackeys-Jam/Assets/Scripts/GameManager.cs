using UnityEngine;
using System.Collections.Generic;
using KinematicCharacterController;
using KinematicCharacterController.Walkthrough.SwimmingState;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float levelDuration = 60f;
    private float levelTimer;

    [Header("Hole Spawning")]
    public List<HoleSpawner> holeSpawners = new List<HoleSpawner>();
    private GameObject Player;
    private int maxActiveHoles;
    public float spawnInterval = 5f;
    private float spawnTimer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Player = GameObject.FindGameObjectWithTag("Player");
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
                WinLevel();
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

    public bool isPlayerOnGround()
    {
        KinematicCharacterMotor Motor = Player.GetComponent<KinematicCharacterMotor>();
        MyCharacterController Character = Player.GetComponent<MyCharacterController>();
        return (Motor.GroundingStatus.IsStableOnGround);
    }

    public bool isPlayerUnderwater()
    {
        MyCharacterController Character = Player.GetComponent<MyCharacterController>();
        return Character.CurrentCharacterState == CharacterState.Swimming;
    }

    public Vector3 getPlayerVelocity()
    {
        KinematicCharacterMotor Motor = Player.GetComponent<KinematicCharacterMotor>();
        return Player.transform.InverseTransformDirection(Motor.Velocity);
    }

    public void LoseLevel()
    {
        Debug.Log("Level Failed!");
        EndLevel();
        // Implement level transition logic here
    }

    public void WinLevel()
    {
        Debug.Log("Level Complete!");
        EndLevel();
        // Implement level transition logic here
    }

    private void EndLevel()
    {
        Debug.Log("Level Ended! Score: " + ScoreManager.Instance.GetScore());
        LeaderboardManager.Instance.AddEntry("N30",Mathf.RoundToInt(ScoreManager.Instance.GetScore()));
        SceneManager.LoadScene(SceneManager.loadedSceneCount+1);
    }
}
