using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class EnemyBall : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform playert;
    public Player player;
    public PlayerCoins playerC;
    public LayerMask isground, isplayer;
    public ParticleSystem explosion;
    public ParticleSystem explosionOnCollision;
    public float health = 100f;
    public int level = 1;
    public float healthScaling = 25f;
    public float rollSpeed = 360f;
    public Transform ray;
    public Transform model;

    public Slider healthbar;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    bool attacked;
    public float damage = 10f;
    public float hitForce = 10f;
    public float attackCooldown = 1.5f;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Healing to Player")]
    public int shieldHealMin = 2;
    public int shieldHealMax = 15;
    public int healMin = 2;
    public int healMax = 10;

    [Header("Player Coin Gain")]
    public int minCoins = 15;
    public int maxCoins = 25;
    public int coinScaling = 5;
    private Collider playerCollider;

    [Header("Explosion Attack")]
    public float explosionRadius = 5f;
    public float explosionDamage = 25f;
    public float explosionForce = 10f;

    private void Awake()
    {
        playert = GameObject.Find("Player").transform;
        player = playert.GetComponent<Player>();
        playerC = playert.GetComponent<PlayerCoins>();
        agent = GetComponent<NavMeshAgent>();

        playerCollider = GameObject.Find("Player").transform.GetComponentInChildren<Collider>();


    }

    private void Start()
    {
        agent.updateRotation = false;

        if (level > 50)
        {
            level = 50;
        }
        if (level > 1)
        {
            health += healthScaling * (level - 1);
            minCoins += coinScaling * (level - 1);
            maxCoins += coinScaling * (level - 1);
        }

        healthbar.maxValue = health;
        healthbar.value = health;

    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, isplayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, isplayer);
        Debug.Log($"Enemy [{name}] sight={playerInSightRange} attack={playerInAttackRange} walkPointSet={walkPointSet} hasPath={agent.hasPath} isOnNavMesh={agent.isOnNavMesh}");

        float dist = Vector3.Distance(transform.position, playert.position);
        if (!playerInSightRange && !playerInAttackRange)
        {
            Wander();
            agent.speed = 3f;
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            Chase();
            if (dist > 10f)
            {
                agent.speed = 6f;
            }
            else
            {
                agent.speed = 3f;
            }
        }

        if (playerInSightRange && playerInAttackRange && LineOfSight())
        {
            Attack();
        }
        float speed = agent.velocity.magnitude;
        //animator.SetBool("Walking", speed > 0.1f);


        healthbar.value = health;
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = healthbar.transform.position.y;

        healthbar.transform.LookAt(camPos);
        healthbar.transform.Rotate(0, 180, 0);
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
            var expl = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(expl.gameObject, 5f);
            Destroy(gameObject);
            HealPlayer();
            playerCoinGain();

        }
    }
    private void Explode()
    {
        var expl = Instantiate(explosionOnCollision, transform.position, Quaternion.identity);
        Destroy(expl.gameObject, 5f);

        player.TakeDamage(explosionDamage, false);


        playerCoinGain();

        Destroy(gameObject);
    }

    public void HealPlayer()
    {
        if (shieldHealMax < shieldHealMin)
        {
            shieldHealMin = shieldHealMax;
        }
        if (healMax < healMin)
        {
            healMin = healMax;
        }

        int shield = Random.Range(shieldHealMin, shieldHealMax);
        int health = Random.Range(healMin, healMax);

        player.Heal(health);
        player.shieldHeal(shield);
    }

    public void playerCoinGain()
    {
        int coins = Random.Range(minCoins, maxCoins);
        playerC.gainCoins(coins);
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
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    private void SearchForWalkpoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, isground))
        {
            walkPointSet = true;
        }
    }
    private void Chase()
    {
        agent.SetDestination(playert.position);
    }
    private void Attack()
    {
        agent.SetDestination(playert.position);

        Vector3 lookPos = playert.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        if (!attacked)
        {
            attacked = true;

            agent.SetDestination(playert.position);

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void LateUpdate()
    {
        Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            Vector3 rollAxis = Vector3.Cross(velocity.normalized, Vector3.up);

            model.Rotate(rollAxis, rollSpeed * Time.deltaTime, Space.World);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((isplayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Explode();
        }
    }

    bool LineOfSight()
    {
        if (playerCollider == null)
        {
            return false;
        }

        Vector3 origin = ray.position;
        Vector3 target = playerCollider.bounds.center;

        Vector3 dir = (target - origin).normalized;
        float dist = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, ~0, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(origin, dir.normalized * dist, Color.red);

            // Walk up the hierarchy and see if this thing belongs to the player
            Player p = hit.transform.GetComponentInParent<Player>();
            return p != null;
        }

        return false;
    }


    private void ResetAttack()
    {
        attacked = false;
    }
}
