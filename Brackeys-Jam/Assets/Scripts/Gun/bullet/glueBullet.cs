using KinematicCharacterController;
using KinematicCharacterController.Walkthrough.SwimmingState;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using static glueBullet;

public class glueBullet : MonoBehaviour
{
    public enum GlueState
    {
        Moving,
        Glued
    }


    public float lifeTime = 10f;
    public float bounceAmount = 2f;
    public float shrinkSpeed = 3f;
    public float maxFlightTime = 0.5f;

    private Collider _SphereCollider;
    private Collider _BoxCollider;
    private Rigidbody _Rigidbody;
    private GlueState _glueState = GlueState.Moving;


    private void Awake()
    {
        _SphereCollider = this.GetComponent<SphereCollider>();
        _BoxCollider = this.GetComponent<BoxCollider>();
        _Rigidbody = this.GetComponent<Rigidbody>();
        _SphereCollider.enabled = true;
        _BoxCollider.enabled = false;
        Invoke("explodeMidAir", maxFlightTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(_glueState)
        {
            case GlueState.Moving:
                if (!collision.gameObject.CompareTag("bullet"))
                {
                    //Destroy(this.GetComponent<Rigidbody>());
                    //place glue bullet at intersection point
                    _Rigidbody.velocity = Vector3.zero;
                    _Rigidbody.isKinematic = true;
                    this.transform.position = collision.GetContact(0).point;
                    //bullet normal lign up with surface normal to orient the model correctly
                    this.transform.up = collision.GetContact(0).normal;
                    StartCoroutine(DestroyAfterTime());
                    _glueState = GlueState.Glued;
                }
                if (collision.gameObject.CompareTag("bullet"))
                {
                    _Rigidbody.AddForce(Bounce(_Rigidbody.velocity.normalized, collision.gameObject.transform.up), ForceMode.Impulse);
                    _Rigidbody.useGravity = true;
                }
                if (collision.gameObject.CompareTag("hole"))
                {
                    Hole hole = collision.gameObject.GetComponent<Hole>();
                    hole.GetHit();
                    Destroy(this.gameObject);
                }
                if (collision.gameObject.CompareTag("enemyBullet"))
                {
                    StartCoroutine(ShrinkAndDestroy());
                    ScoreManager.Instance.RegisterAction("Bullet Defflected");
                }
                break;

            case GlueState.Glued:
                /*Debug.Log("Player Bounced !");
                Vector3 direction = collision.gameObject.GetComponent<KinematicCharacterMotor>().Velocity.normalized;
                collision.gameObject.GetComponent<MyCharacterController>().AddVelocity(Bounce(direction, transform.up));*/
                break;
        }
    }

    public Vector3 Bounce(Vector3 initialDirection, Vector3 normal)
    {
        return (Vector3.up + normal).normalized * bounceAmount;
    }

    public GlueState GetGlueState()
    {
        return _glueState;
    }

    private void explodeMidAir()
    {
        if (_glueState == GlueState.Moving)
        {
            StartCoroutine(ShrinkAndDestroy());
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        StartCoroutine(ShrinkAndDestroy());
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
        Destroy(this.gameObject);
    }
}
