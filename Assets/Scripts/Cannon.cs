using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class Cannon : MonoBehaviour
{

    [Header("Inspector Elements")]
    public Camera cam;
    public ParticleSystem muzzleFlash;
    public GameObject impact;
    public Player player;
    public LayerMask playerMask;
    [Header("Levels")]
    public int level = 1;
    public float levelDamageScaling = 1f;
    public float levelfirerateScaling = 1f;
    public float levelcostReduction = 0f;
    public bool maxLevel = false;
    public bool fullyLeveled = false;
    public float maxLevelDmgBoost = 2f;
    public float maxLevelCostReduction = 0f;
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float firerate = 2f;
    private float firenext = 0f;
    private bool isFiring = false;
    public float healthCost = 5f;
    public AudioSource shootSound;
    private float baseFirerate;
    public float baseDamage;
    public GameObject leftCannon;
    public bool isLeftCannon = false;
    [Header("Explosive Settings")]
    public bool explosive = false;
    public float explosionRadius = 2f;
    [Header("UI Elements")]
    public Slider cooldownBar;
    private Image cooldownFill;
    private CanvasGroup cooldownGroup;
    private float hideTimer = 0f;

    private float lastShotTime;
    private bool shotOnce = false;

    private void Start()
    {
        ScaleLevel(1);
        cooldownGroup = cooldownBar.GetComponent<CanvasGroup>();
        cooldownFill = cooldownBar.fillRect.GetComponent<Image>();
        lastShotTime = 0;
        baseFirerate = firerate;
        baseDamage = damage;
        if (!isLeftCannon)
        {
            leftCannon.SetActive(false);
        }

    }
    void Update()
    {
        if (isFiring && Time.time >= firenext) 
        {
            firenext = Time.time + 1f / firerate; 
            Shoot(); 
        }
        float cooldown = 1f / firerate;
        float t = Mathf.Clamp01((Time.time - lastShotTime) / cooldown);
        if (!shotOnce)
        {
            cooldownBar.value = 1f;
        }
        else
        {
            cooldownBar.value = t;
        }
        cooldownFill.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), Color.white, t);
        if (t >= 1f)
        {
            hideTimer += Time.deltaTime;

            if (hideTimer >= 0.5f)
            {
                cooldownGroup.alpha = Mathf.Lerp(cooldownGroup.alpha, 0f, Time.deltaTime * 5f);
            }
        }
        else
        {
            hideTimer = 0f;
            cooldownGroup.alpha = 1f;
        }



    }

    public void OnAttack(InputValue value)
    {
        if (Time.timeScale < 0.1f)
        {
            isFiring = false;
            return;
        }

        isFiring = value.isPressed;
    }

    void Shoot()
    {
        lastShotTime = Time.time;
        shotOnce = true;
        if (healthCost <= 0)
        {
            healthCost = 0;
        }
        //TakeDamage(health value, shieldbypass (true or false))
        player.TakeDamage(healthCost, true, false);

        muzzleFlash.Play();
        RaycastHit hit;
        shootSound.Play();
        // ~ means everything but the chosen value so ~playerMask allows the raycast to ignore the player
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, ~playerMask))
        {

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            EnemyBall enemyb = hit.transform.GetComponent<EnemyBall>();
            BossEnemy enemyBoss = hit.transform.GetComponent<BossEnemy>();
            if (!explosive)
            {
                // Normal hitscan damage
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                    
                else if (enemyb != null)
                {
                    enemyb.TakeDamage(damage);
                } else if(enemyBoss != null)
                {
                    enemyBoss.TakeDamage(damage);
                }
                    
            }
            else
            {
                // Explosive AOE damage
                CreateExplosion(hit.point);
            }

            GameObject impactobj = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactobj, 2f);
        }
    }

    void CreateExplosion(Vector3 position)
    {
        HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();
        HashSet<EnemyBall> damagedBalls = new HashSet<EnemyBall>();

        if (impact != null)
        {
            GameObject boom = Instantiate(impact, position, Quaternion.identity);
            Destroy(boom, 3f);
        }

        Collider[] hits = Physics.OverlapSphere(position, explosionRadius);

        foreach (Collider col in hits)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            EnemyBall enemyb = col.GetComponent<EnemyBall>();
            BossEnemy enemyBoss = col.transform.GetComponent<BossEnemy>();
            if (enemy != null && !damagedEnemies.Contains(enemy))
            {
                damagedEnemies.Add(enemy);
                enemy.TakeDamage(damage);
            }
            else if (enemyb != null && !damagedBalls.Contains(enemyb))
            {
                damagedBalls.Add(enemyb);
                enemyb.TakeDamage(damage);
            }
            else if (enemyBoss != null)
            {
                enemyBoss.TakeDamage(damage);
            }
        }

    }


    public void ScaleLevel(int lvl)
    {
        if (isLeftCannon)
        {
            return;
        }
        //max level check to not infinitely scale weapon damage
        if (lvl >= 10)
        {
            level = 10;
            maxLevel = true;
        }

        if (maxLevel && !fullyLeveled)
        {
            firerate = baseFirerate + (levelfirerateScaling * (level - 1));
            damage = baseDamage + (levelDamageScaling * (level - 1)) * maxLevelDmgBoost;
            if (healthCost > 0)
            {
                healthCost -= Mathf.Floor(maxLevelCostReduction);
            }
            fullyLeveled = true;
        }
        else if (lvl > 1 && !maxLevel)
        {
            level = lvl;
            firerate = baseFirerate + (levelfirerateScaling * (level - 1));
            damage = baseDamage + (levelDamageScaling * (level - 1));
            if(healthCost > 0)
            {
                healthCost -= Mathf.Floor((levelcostReduction * (level - 1)));
            }
            
        }


    }
}
