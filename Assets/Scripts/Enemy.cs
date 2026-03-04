using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask isground, isplayer;
    public float health = 100f;

    public GameObject projectile;
    public Transform shootPoint;


    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float attackRate;
    bool attacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, isplayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, isplayer);

        if(!playerInSightRange && !playerInAttackRange)
        {
            Wander();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            Chase();
        }
        if (playerInSightRange && playerInAttackRange)
        {
            Attack();
        }
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }

        void Die()
        {
            Destroy(gameObject);
        }
    }

    private void Wander()
    {
        if (!walkPointSet)
        {
            SearchForWalkpoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    private void SearchForWalkpoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, isground))
        {
            walkPointSet = true;
        }
    }
    private void Chase()
    {
        agent.SetDestination(player.position);
    }
    private void Attack()
    {
        agent.SetDestination(transform.position);

        Vector3 lookPos = player.position; 
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        if (!attacked)
        {
            GameObject proj = Instantiate(projectile, shootPoint.position, Quaternion.identity);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 1f, ForceMode.Impulse);

            attacked = true;
            Invoke(nameof(ResetAttack), attackRate);
            Destroy(proj, 2f);
        }
    }
    private void ResetAttack()
    {
        attacked = false;
    }
}
