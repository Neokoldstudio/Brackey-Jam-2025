using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    // Start is called before the first frame update
    public float minScale = 0.002f;
    public float maxScale = 0.0155f;
    public float timeDisapear = 0.1f;

    void Start()
    {
        float scale = Random.Range(minScale, maxScale);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Random.Range(0, 360));
        transform.localScale = new Vector3(scale,scale,scale);
        StartCoroutine(Disapear());  
    }

    private IEnumerator Disapear()
    {
        yield return new WaitForSeconds(timeDisapear);
        Destroy(gameObject);
    }
}
