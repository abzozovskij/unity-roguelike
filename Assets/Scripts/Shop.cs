using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class Shop : MonoBehaviour, IInteractable
{
    [Header("UI")]
    [SerializeField] private GameObject shopUI;
    private PlayerInput playerInput;
    public GameObject crosshair;
    public GameObject cdBar;
    public TextMeshProUGUI healText;
    public TextMeshProUGUI healAmountText;
    public Cannon[] weapons;
    public PlayerCoins coins;
    public int upgradeCost;
    public int powerCost;
    public int level;
    public TextMeshProUGUI upgradeText;
    public TextMeshProUGUI powerText;
    public int healCost = 100;
    public Player player;
    public int healAmount = 50;
    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        weapons = FindObjectsByType<Cannon>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        coins = FindFirstObjectByType<PlayerCoins>();
        player = FindFirstObjectByType<Player>();
        Debug.Log(player);
    }

    private void Start()
    {
        healAmountText.text = "For " + healAmount.ToString();
        upgradeCost = 100;
        powerCost = 200;
        level = weapons[0].level;
    }
    public void Interact()
    {
        OpenShop();
    }

    private void OpenShop()
    {
        shopUI.SetActive(true);
        crosshair.SetActive(false);
        cdBar.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerInput.SwitchCurrentActionMap("UI");
        Time.timeScale = 0f;
    }

    public void UpgradeWeapon()
    {
        if(coins.coins >= upgradeCost)
        {
            foreach (Cannon weapon in weapons)
            {
                weapon.ScaleLevel(level + 1);
            }

            coins.useCoins(upgradeCost);
            level += 1;
            upgradeCost += 100;
            upgradeText.text = upgradeCost.ToString();
        }
        else
        {
            return;
        }
    }

    public void Heal()
    {
        if (coins.coins >= healCost)
        {
            player.Heal(healAmount);
            coins.useCoins(healCost);
            healText.text = healCost.ToString();
            
        }
        else if(player.health >= player.maxHealth)
        {
            return;
        }
        return;
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        crosshair.SetActive(true);
        cdBar.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput.SwitchCurrentActionMap("Player");
        Time.timeScale = 1f;
    }
}
