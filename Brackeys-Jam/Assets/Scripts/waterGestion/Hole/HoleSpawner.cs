using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSpawner : MonoBehaviour
{
    public GameObject holePrefab;
    public GameObject Opened;
    public GameObject Closed;
    private GameObject currentHole;

    public void Start()
    {
        Opened.SetActive(false);
        Closed.SetActive(true);
    }

    public bool SpawnHole()
    {
        if (currentHole == null)
        {
            Opened.SetActive(true);
            Closed.SetActive(false);

            currentHole = Instantiate(holePrefab, transform.position, Quaternion.identity);
            currentHole.transform.rotation = this.transform.rotation;
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

    public void SealHole()
    {
        if (currentHole != null)
        {
            Opened.SetActive(false);
            Closed.SetActive(true);
        }
    }

    public bool HasHole() { return currentHole != null; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,1f);
    }
}