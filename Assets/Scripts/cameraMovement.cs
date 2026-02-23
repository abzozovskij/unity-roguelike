using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    public Transform cameraPos;

    void Update()
    {
        transform.position = cameraPos.position;
    }
}
