using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform playert;
    public Player player;
    public PlayerCoins playerC;
    public LayerMask isground, isplayer;
    public ParticleSystem explosion;

    public float health = 100f;
    public int level = 1;
    public float healthScaling = 25f;

    public Animator animator;

    public bool largeMech = false;
    public float attackCooldown = 1.5f;
    private float cooldownTimer;
    private float timesAttacked = 0f;
    private bool onCooldown = false;

    public GameObject projectile;
    public Transform[] shootPoints;
    private int shootIndex = 0;

    public Transform ray;
    public Slider healthbar;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float attackRate;
    bool attacked;
    public float attackForceH = 32f;
    public float attackForceV = 1f;

    public float sightRange, attackRange, runRange;
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

    private void Awake()
    {
        playert = GameObject.Find("Player").transform;
        player = playert.GetComponent<Player>();
        playerC = playert.GetComponent<PlayerCoins>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

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
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(agent.velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
        float dist = Vector3.Distance(transform.position, playert.position);
        if (!playerInSightRange && !playerInAttackRange)
        {
            Wander();
            agent.speed = 3f;
            animator.SetFloat("Speed", 0f);

        }
        if (playerInSightRange && !playerInAttackRange)
        {
            Chase();
            if (dist > runRange)
            {
                agent.speed = 6f;
                animator.SetFloat("Speed", 1f);
            }
            else
            {
                agent.speed = 3f;
                animator.SetFloat("Speed", 0f);
            }
        }

        if (playerInSightRange && playerInAttackRange && LineOfSight() && !onCooldown)
        {
            Attack();
        }
        float speed = agent.velocity.magnitude;
        animator.SetBool("Walking", speed > 0.1f);


        healthbar.value = health;
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = healthbar.transform.position.y;

        healthbar.transform.LookAt(camPos);
        healthbar.transform.Rotate(0, 180, 0);
        if (onCooldown)
        {
            Vector3 lookPos = playert.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                onCooldown = false;
            }
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
            var expl = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(expl.gameObject, 5f);
            Destroy(gameObject);
            HealPlayer();
            playerCoinGain();
            
        }
    }
    
    public void HealPlayer()
    {
        if(shieldHealMax < shieldHealMin)
        {
            shieldHealMin = shieldHealMax;
        } 
        if(healMax < healMin)
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

        agent.SetDestination(playert.position);
    }
    private void Attack()
    {

        if (largeMech)
        {
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
                return;
            }
        }
        agent.SetDestination(transform.position);

        Vector3 lookPos = playert.position; 
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        if (!attacked)
        {
            Transform shootPoint = shootPoints[shootIndex];
            Vector3 dir = (playerCollider.bounds.center - shootPoint.position).normalized;
            GameObject proj = Instantiate(projectile, shootPoint.position, Quaternion.identity);
            proj.GetComponent<Projectile>().SetEnemy(this);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.AddForce(dir * attackForceH, ForceMode.Impulse);
            rb.AddForce(transform.up * attackForceV, ForceMode.Impulse);

            Destroy(proj, 2f);

            shootIndex++;
            if (largeMech)
            {
                timesAttacked++;
            }
            if (shootIndex >= shootPoints.Length)
            {
                shootIndex = 0;
            }

            attacked = true;
            Invoke(nameof(ResetAttack), attackRate);

            if (largeMech && timesAttacked > 16) {
                cooldownTimer = attackCooldown;
                timesAttacked = 0;
                onCooldown = true;
            }
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
