using UnityEngine;
[CreateAssetMenu(fileName = "HealthRegen", menuName = "Scriptable Objects/Powers/Health Regen")]
public class HealthRegen : Power
{
    public float amount;

    public override void Apply()
    {
        Player player = FindFirstObjectByType<Player>();
        player.healthRegen += amount;
    }
}
