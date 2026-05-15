using UnityEngine;
using UnityEngine.InputSystem;
public class Completion : MonoBehaviour
{
    public GameObject completionScreen;
    public bool completed = false;
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            completed = true;
            completionScreen.SetActive(true);
            Time.timeScale = 0f;
            playerInput.SwitchCurrentActionMap("UI");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }
}
