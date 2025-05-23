using KinematicCharacterController;
using KinematicCharacterController.Walkthrough.SwimmingState;
using System.Collections;
using System.Collections.Generic;
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

    public GameObject impactVfx;
    private Collider _SphereCollider;
    private Collider _BoxCollider;
    private Rigidbody _Rigidbody;
    private Animator _anim;
    private TrailRenderer _TrailRenderer;

    private GlueState _glueState = GlueState.Moving;


    private void Awake()
    {
        _anim = gameObject.GetComponent<Animator>();
        _SphereCollider = this.GetComponent<SphereCollider>();
        _BoxCollider = this.GetComponent<BoxCollider>();
        _Rigidbody = this.GetComponent<Rigidbody>();
        _TrailRenderer = this.GetComponent<TrailRenderer>();
        _SphereCollider.enabled = true;
        _BoxCollider.enabled = false;
        _anim.enabled = false;
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
                    GameObject slimeVfx = Instantiate(impactVfx, transform);
                    _glueState = GlueState.Glued;
                    StartBounceAnimation();
                    _TrailRenderer.enabled = false;
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.glueGunHit, this.transform.position);
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
                if (collision.gameObject.CompareTag("gluedEnemy"))
                {
                    StartCoroutine(ShrinkAndDestroy());
                }
                if (collision.gameObject.CompareTag("water"))
                { 
                    _Rigidbody.velocity = _Rigidbody.velocity.normalized * 10f;
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

    public void StartBounceAnimation() 
    {
        _anim.enabled = true;
        _anim.SetTrigger("Bounce");
    }

    public void deactivateAnimator()
    {
        _anim.enabled = false;
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
