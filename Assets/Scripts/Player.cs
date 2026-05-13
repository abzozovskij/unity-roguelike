using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : MonoBehaviour
{  
    [Header("Player Attributes")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float shield = 25f;
    public float maxShield = 25f;
    public float healthRegen = 1f;
    public float regenInterval = 2f;

    private float regenTimer = 0f;

    [Header("UI Elements")]
    public Slider healthBar;
    public Slider shieldBar;
    public TextMeshProUGUI healthValue;
    public GameObject youDiedText;
    public Image damagedEffect;
    public float damageEffectSpeed = 5f;
    public float damagedAlpha = 0.4f;

    private Color overlayColour;
    private bool effectActive = false;

    [Header("Misc")]
    public GameObject playerweapon;
    public movement movement;
    void Start()
    {
 
        health = maxHealth;
        healthBar.maxValue = maxHealth;
 
        shield = maxShield;
        shieldBar.maxValue = maxShield;

    }


    void Update()
    {
        regenTimer += Time.deltaTime;
        if(regenTimer >= regenInterval)
        {
            regenTimer = 0f;
            if (health < maxHealth)
            {
                health += healthRegen;
                health = Mathf.Clamp(health, 0, maxHealth);
            }
        }


        healthBar.value = health;
        healthBar.maxValue = maxHealth;
        shieldBar.value = shield;

        if (shield > 0)
        {
            healthValue.text = (health)  + " / " + maxHealth + " ( + " + shield + " )";
            healthValue.color = new Color32(64, 252, 255, 255);
        }
        else
        {
            healthValue.text = health + " / " + maxHealth;
            healthValue.color = Color.white;
        }

        if (effectActive)
        {
            overlayColour = damagedEffect.color;
            overlayColour.a = Mathf.Lerp(overlayColour.a, 0f, damageEffectSpeed * Time.deltaTime);
            damagedEffect.color = overlayColour;

            if (overlayColour.a <= 0.01f)
            {
                overlayColour.a = 0f;
                damagedEffect.color = overlayColour;
                effectActive = false;
            }
        }

    }

    public void TakeDamage(float dmg, bool shieldbypass, bool damageEffectActive = true)
    {   
        //invulnerable during dash
        if (movement.dash)
        {
            return;
        }
        if (damageEffectActive == true){
            // Trigger damage flash
            overlayColour = damagedEffect.color;
            overlayColour.a = damagedAlpha;
            damagedEffect.color = overlayColour;
            effectActive = true;

        }
       
        //shield check
        if (shield <= 0)
        {
            shield = 0;
            health -= dmg;
        }
        //player attacks bypass shields by default
        else if (shieldbypass)
        {
            health -= dmg;
        }
        else
        {
            shield -= dmg;
        }

        if(health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        if(health >= maxHealth)
        {
            health = maxHealth;
        }
    }
    public void shieldHeal(int amount)
    {
        shield += amount;
        if (shield >= maxShield)
        {
            shield = maxShield;
        }
    }

    public void Die()
    {
        youDiedText.SetActive(true);
        Time.timeScale = 0f;
        Destroy(playerweapon);
    }
}
