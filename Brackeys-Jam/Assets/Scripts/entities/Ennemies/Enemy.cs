using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : Entity
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float wanderRadius = 10f;
    public float obstacleAvoidanceDistance = 1f;
    public float attackRange = 15f;
    public float attackCooldown = 2f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;
    public GameObject bulletPrefab;
    public float shootForce = 10f;
    public GameObject gluedEnemy;

    private Material material;
    private Transform player;
    private Vector3 wanderTarget;
    private bool canAttack = true;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().GetPlayerTarget();
        material = GetComponentInChildren<Renderer>().material;
        SetNewWanderTarget();
    }

    protected virtual void Update()
    {
        if (CanSeePlayer() && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (canAttack && !isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            Wander();
        }
    }

    public override void GetHit(float damage)
    {
        StartCoroutine(Flash());
        base.GetHit(damage);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyHit, this.transform.position);
    }

    private void Wander()
    {
        if (Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            SetNewWanderTarget();
        }

        Vector3 direction = (wanderTarget - transform.position).normalized;
        if (Physics.Raycast(transform.position, direction, obstacleAvoidanceDistance, obstacleMask))
        {
            AvoidObstacles(ref direction);
        }
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
        transform.forward = direction;
    }

    private void SetNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        wanderTarget = randomDirection;
    }

    private void AvoidObstacles(ref Vector3 moveDirection)
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(rayDirections[i]);

            if (!Physics.Raycast(transform.position, dir, obstacleAvoidanceDistance, obstacleMask))
            {
                moveDirection = dir.normalized; // Adjust movement to the first clear path
                return;
            }
        }

        moveDirection = -transform.forward; // If all directions are blocked, back up
    }

    protected override void Die()
    {
        // Death logic (e.g., play death animation, drop loot, etc.)
        Debug.Log("Enemy died!");

        if (ScoreManager.Instance.GetAirTime()>=1.0f)
        {
                ScoreManager.Instance.RegisterAction("Airborne Kill");
        }
        else ScoreManager.Instance.RegisterAction("Kill");

        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDeath, this.transform.position);

        WaterSystem.Instance.DrainWater(10f);
        base.Die();
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, attackRange, playerMask))
        {
            return hit.collider.CompareTag("Player");
        }
        return false;
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;
        // Attack logic (e.g., instantiate bullet, play animation, etc.)
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0));
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        transform.LookAt(player.position);

        if (bulletRb)
        {
            bulletRb.AddForce(direction * shootForce, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            Vector3 direction = (transform.position - collision.gameObject.transform.position).normalized;
            GameObject glued = Instantiate(gluedEnemy, collision.gameObject.transform.position, Quaternion.identity);
            Rigidbody gluedRb = glued.GetComponent<Rigidbody>();

            if (gluedRb)
            {
                Debug.Log("force added");
                gluedRb.AddForce(collision.relativeVelocity, ForceMode.Impulse);
            }
            ScoreManager.Instance.RegisterAction("Ennemy Glued !");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private IEnumerator Flash()
    {
        Color originalColor = material.color;
        material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        material.color = originalColor;
    }
}
