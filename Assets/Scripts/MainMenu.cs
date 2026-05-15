using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject controls;
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
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
}
