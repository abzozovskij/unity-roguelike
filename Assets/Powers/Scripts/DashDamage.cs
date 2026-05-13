using UnityEngine;
[CreateAssetMenu(fileName = "DashDamage", menuName = "Scriptable Objects/Powers/Dash Damage")]
public class DashDamage : Power
{
    private bool firstAbility = true;
    public float baseAmount;
    public float upgradeAmount;
    private string originalDescription;

    private void OnEnable()
    {
        originalDescription = "Deal " + baseAmount + " damage when dashing into enemies";
        
    }
    private void OnDisable()
    {
        description = originalDescription;
        firstAbility = true;
    }

    public override void Apply()
    {
        if (firstAbility)
        {
            movement playerm = FindFirstObjectByType<movement>();
            playerm.dashDamageTrigger = true;
            playerm.dashDamage = baseAmount;
            firstAbility = false;
            description = "Deal an additional " + upgradeAmount + "% damage to when dashing into enemies";
        }
        else { 
            movement playerm = FindFirstObjectByType<movement>();
            playerm.dashDamageTrigger = true;
            
            playerm.dashDamage *= 1 + (upgradeAmount / 100f);
        }

    }
}
