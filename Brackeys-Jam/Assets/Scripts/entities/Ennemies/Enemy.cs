using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
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


    private Transform player;
    private Vector3 wanderTarget;
    private bool canAttack = true;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().GetPlayerTarget();
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

        if (!GameManager.Instance.isPlayerOnGround())
        {
            if (!GameManager.Instance.isPlayerUnderwater())
            {
                ScoreManager.Instance.RegisterAction("Airborne Kill");
            }
            else ScoreManager.Instance.RegisterAction("Kill");
        }
        else ScoreManager.Instance.RegisterAction("Kill");

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
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(direction));
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

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
            GameObject glued = Instantiate(gluedEnemy, transform.position, Quaternion.identity);
            Rigidbody gluedRb = glued.GetComponent<Rigidbody>();

            if (gluedRb)
            {
                gluedRb.velocity = direction * 10f;
            }
            ScoreManager.Instance.RegisterAction("Ennemy Glued !");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
