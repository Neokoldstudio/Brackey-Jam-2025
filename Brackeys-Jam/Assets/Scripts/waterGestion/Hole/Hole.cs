using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private HoleSpawner holeSpawner;
    public float shrinkSpeed = 3f;

    private void Awake()
    {
        WaterSystem.Instance.RegisterHole();
    }

    public void GetHit()
    {
        Die();
    }

    public void Die()
    {
        StartCoroutine(ShrinkAndDestroy());
        if (ScoreManager.Instance.GetAirTime() > 1.0f)
        { 
            ScoreManager.Instance.RegisterAction("Airborne Plucking!");

        }
        ScoreManager.Instance.RegisterAction("Hole Plucked");
    }

    public void SetHoleSpawner(HoleSpawner holeSpawner)
    {
        this.holeSpawner = holeSpawner;
    }

    private void OnDestroy()
    {
        WaterSystem.Instance.UnregisterHole();
    }

    private IEnumerator ShrinkAndDestroy()
    {
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * shrinkSpeed;
            this.transform.localScale = Vector3.one * (1 - timer);
            yield return null;
        }
        holeSpawner.RemoveHole();
    }
}