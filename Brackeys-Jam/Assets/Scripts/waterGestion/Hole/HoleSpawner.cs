using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSpawner : MonoBehaviour
{
    public GameObject holePrefab;
    private GameObject currentHole;

    public bool SpawnHole()
    {
        if (currentHole == null)
        {
            currentHole = Instantiate(holePrefab, transform.position, Quaternion.identity);
            currentHole.GetComponent<Hole>().SetHoleSpawner(this);
            return true;
        }
        return false;
    }

    public void RemoveHole()
    {
        if (currentHole != null)
        {
            Destroy(currentHole);
            currentHole = null;
        }
    }

    public bool HasHole() { return currentHole != null; }
}