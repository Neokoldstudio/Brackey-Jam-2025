using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gluedEnemy :Entity
{

    public GameObject Enemy;
    public Transform shell;
    public float explosionRadius = 5f;
    public float explosionDamage = 700f;
    public float playerExplosionDamage = 20f;
    public float unglueTime = 10f;
    // Start is called before the first frame update

    public void Start()
    {
        StartCoroutine(UnglueAfterTime());
        Debug.Log("bonjour");
    }

    public override void GetHit(float damage)
    {
        base.GetHit(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hole")
        {
            collision.gameObject.GetComponent<Hole>().GetHit();
            ScoreManager.Instance.RegisterAction("Hole Plucked With Enemy !!");
            Die();
        }
    }

    private IEnumerator UnglueAfterTime()
    {
        Vector3 originalScale = shell.localScale;
        float timer = 0;

        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;

        while (timer < unglueTime)
        {
            timer += Time.deltaTime;
            shell.localScale = Vector3.Lerp(originalScale, Vector3.zero, timer / unglueTime);
            yield return null;
        }

        Instantiate(Enemy, transform.position, Quaternion.identity);
        Die();
    }
}
