using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    public GameObject airEnemy;
    public GameObject waterEnemy;

    public float spawnRate = 5f;
    public float spawnProbability = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    public void SpawnAirEnemy()
    {
        Instantiate(airEnemy, transform.position, Quaternion.identity);
    }

    public void SpawnWaterEnemy()
    {
        Instantiate(waterEnemy, transform.position, Quaternion.identity);
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            bool isUnderWater = transform.position.y < WaterSystem.Instance.getWaterPlaneHeight();
            if (Random.value < spawnProbability)
            {
                if (isUnderWater) { SpawnWaterEnemy(); }
                else SpawnAirEnemy();
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }
}
