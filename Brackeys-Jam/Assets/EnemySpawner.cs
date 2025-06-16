using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;

    public GameObject spawnArea;
    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            GameObject newEnemy = Instantiate(enemy, spawnArea.transform.position, Quaternion.identity);
        }
    }
}
