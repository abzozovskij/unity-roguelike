using UnityEngine;
[CreateAssetMenu(fileName = "DashUp", menuName = "Scriptable Objects/Powers/Dash Up")]
public class DashUp : Power
{
    public int amount;

    public override void Apply()
    {
        movement playerm = FindFirstObjectByType<movement>();
        playerm.AddDashes(amount);
    }
}
