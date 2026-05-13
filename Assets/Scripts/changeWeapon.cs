using UnityEngine;

public class changeWeapon : MonoBehaviour, IInteractable
{
    public GameObject[] weapons;
    private int currentWeapon = 0;
    public void Interact()
    {
        SwitchWeapon();
    }

    public void SwitchWeapon()
    {
        foreach(GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        currentWeapon++;
        if(currentWeapon >= weapons.Length)
        {
            currentWeapon = 0;
        }
        weapons[currentWeapon].SetActive(true);
    }
}
