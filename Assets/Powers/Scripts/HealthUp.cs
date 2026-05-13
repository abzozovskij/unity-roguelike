using UnityEngine;

[CreateAssetMenu(fileName = "HealthUp", menuName = "Scriptable Objects/Powers/Health Up")]
public class HealthUp : Power
{

    public float amount;

    public override void Apply()
    {
        Player player = FindFirstObjectByType<Player>();
        float oldMax = player.maxHealth;
        player.maxHealth = Mathf.Floor(player.maxHealth * (1 + amount / 100f));
        float hpgain = player.maxHealth - oldMax;
        player.health += hpgain;
    }
}
