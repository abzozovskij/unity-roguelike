using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : MonoBehaviour
{  
    [Header("Player Attributes")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float healthRegen = 1f;
    public float regenInterval = 2f;

    private float regenTimer = 0f;

    [Header("UI Elements")]
    public Slider healthBar;
    public TextMeshProUGUI healthValue;
    public GameObject youDiedText;

    [Header("Misc")]
    public GameObject playerweapon;
    void Start()
    {
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthValue.text = health + " / " + maxHealth + " HEALTH";
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
        healthValue.text = health + " / " + maxHealth + " HEALTH";
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        youDiedText.SetActive(true);
        Time.timeScale = 0f;
        Destroy(playerweapon);
    }
}
