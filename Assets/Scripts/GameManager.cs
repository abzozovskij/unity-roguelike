using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    public List<Power> powers;
    public GameObject[] powerSelections;
    private Power[] choices;
    private PlayerInput playerInput;
    public GameObject powersUI;
    public GameObject crosshair;
    public GameObject cdBar;

    private void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
    }
    public void ApplyPower(Power power)
    {
        power.Apply();

    }

    public void ShowRandomPowers()
    {
        choices = new Power[3];
        List<Power> pool = new List<Power>(powers);

        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, pool.Count);
            choices[i] = pool[index];
            pool.RemoveAt(index);
        }

        UpdatePowerUI();
    }

    private void UpdatePowerUI()
    {
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        crosshair.SetActive(false);
        cdBar.SetActive(false);
        powersUI.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            Power p = choices[i];
            Transform bar = powerSelections[i].transform;

            Image icon = bar.Find("Icon").GetComponent<Image>();
            TMP_Text name = bar.Find("Title").GetComponent<TMP_Text>();
            TMP_Text desc = bar.Find("Description").GetComponent<TMP_Text>();

            icon.sprite = p.icon;
            name.text = p.powerName;
            desc.text = p.description;
        }
    }

    public void OnPowerClicked(int index)
    {
        powersUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crosshair.SetActive(true);
        cdBar.SetActive(true);
        Time.timeScale = 1f;
        ApplyPower(choices[index]);
        playerInput.SwitchCurrentActionMap("Player");
    }


    void Update()
    {
        
    }
}
