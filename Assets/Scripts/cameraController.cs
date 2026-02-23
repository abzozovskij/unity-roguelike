using UnityEngine;
using UnityEngine.InputSystem;
public class cameraController : MonoBehaviour
{

    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRot;
    float yRot;
    private Vector2 look;
    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = look.x * sensX * Time.deltaTime;
        float mouseY = look.y * sensY * Time.deltaTime;
        yRot += mouseX;
        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
        //Debug.Log(look);
    }
}
