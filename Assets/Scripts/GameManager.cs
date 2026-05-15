using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public List<Power> powers;
    public GameObject[] powerSelections;
    private Power[] choices;
    private PlayerInput playerInput;
    public GameObject powersUI;
    public GameObject crosshair;
    public GameObject cdBar;
    public Player player;
    public AudioSource musicSource;
    private InputAction pauseAction;
    public AudioClip normalMusic;
    public AudioClip combatMusic;
    AudioLowPassFilter lp;
    public Completion completion;
    public GameObject pauseMenu;
    public GameObject controls;
    private bool paused = false;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        player = FindFirstObjectByType<Player>();
        PlayNormalMusic();
        lp = musicSource.GetComponent<AudioLowPassFilter>();
        playerInput = FindFirstObjectByType<PlayerInput>();
        completion = FindFirstObjectByType<Completion>();
        pauseAction = playerInput.actions["Pause"];
        Debug.Log(lp);


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
        player.maxHealth = Mathf.Ceil(player.maxHealth * 1.10f);
        player.health = Mathf.Ceil(player.health * 1.10f);
        Time.timeScale = 1f;
        ApplyPower(choices[index]);
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void PlayNormalMusic()
    {
        musicSource.clip = normalMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayCombatMusic()
    {
        musicSource.clip = combatMusic;
        musicSource.loop = true;
        musicSource.Play();
    }


    public void PauseGame()
    {
        if (player.died || completion.completed)
        {
            return;
        }

        lp.cutoffFrequency = 600f;
        Time.timeScale = 0.09f;
        paused = true;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        

    }
    public void ResumeGame()
    {
        if (player.died || completion.completed)
        {
            return;
        }

        lp.cutoffFrequency = 22000f;
        Time.timeScale = 1f;
        paused = false;
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        controls.SetActive(false);
        
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ShowControls()
    {
        controls.SetActive(true);
    }

    public void CloseControls()
    {
        controls.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void OnPause()
    {
        if (paused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    void Update()
    {
        if (pauseAction.triggered)
        {
            if (paused)
            {
                ResumeGame();
            }

            else
            {
                PauseGame();
            }
                
        }
    }


}
