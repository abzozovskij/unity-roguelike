using UnityEngine;
[CreateAssetMenu(fileName = "ExtraFirepower", menuName = "Scriptable Objects/Powers/Extra Firepower")]
public class ExtraFirepower : Power
{
    private bool firstAbility = true;
    public float upgradeAmount;
    private string originalDescription;

    private void OnEnable()
    {
        originalDescription = "Obtain a second weapon that does not drain life when shooting";

    }
    private void OnDisable()
    {
        description = originalDescription;
        firstAbility = true;
    }

    public override void Apply()
    {
        Cannon[] cannons = FindObjectsByType<Cannon>(FindObjectsSortMode.None);
        Cannon leftC = null;
        Cannon rightC = null;
        foreach (var cannon in cannons)
        {
            if (cannon.isLeftCannon)
            {
                leftC = cannon;
            }
            else
            {
                rightC = cannon;
            }
        }

        if (firstAbility)
        {
            rightC.leftCannon.SetActive(true);
            firstAbility = false;
            description = "Deal an additional " + upgradeAmount + "% damage with your second cannon";
        }
        else
        {
            if (leftC == null)
            {
                return;
            }
            leftC.damage *= 1 + (upgradeAmount / 100f);
        }

    }
}
