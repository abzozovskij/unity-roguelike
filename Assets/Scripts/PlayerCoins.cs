using UnityEngine;
using TMPro;
public class PlayerCoins : MonoBehaviour
{

    public int coins = 0;
    public TextMeshProUGUI cointext;

    public void gainCoins(int amount)
    {
        coins += amount;

    }

    public void useCoins(int amount)
    {
        if(coins >= amount)
        {
            coins -= amount;
        }
        else
        {
            return;
        }
        
    }

    private void Update()
    {
        cointext.text = "Coins: " + coins;
    }
}
