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
        Explode();
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

    private void Explode()
    {
        //Instantiate(Enemy, transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.tag == "enemies")
            {
                nearbyObject.GetComponent<PlayerManager>().GetHit(explosionDamage);
            }
            if (nearbyObject.tag == "Player")
            { 
                nearbyObject.GetComponent<PlayerManager>().GetHit(playerExplosionDamage);
            }
        }
        ScoreManager.Instance.RegisterAction("Boom !!");
        Die();
    }

    private IEnumerator UnglueAfterTime()
    {
        float timer = 0;

        while (timer < unglueTime)
        {
            timer += Time.deltaTime;
            shell.localScale = Vector3.one * (1 - (timer / unglueTime));
            yield return null;
        }

        Instantiate(Enemy, transform.position, Quaternion.identity);
        Die();
    }
}
