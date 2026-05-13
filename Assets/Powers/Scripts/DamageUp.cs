using UnityEngine;

[CreateAssetMenu(fileName = "DamageUp", menuName = "Scriptable Objects/Powers/Damage Up")]
public class DamageUp : Power
{
    public float amount;

    public override void Apply()
    {
        Cannon[] cannons = FindObjectsByType<Cannon>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Cannon cannon in cannons)
        {

            cannon.damage *= 1 + (amount / 100f);
            cannon.baseDamage *= 1 + (amount / 100f);
            cannon.levelDamageScaling *= 1 + (amount / 100f);
        }
    }
}
