using UnityEngine;
using UnityEngine.InputSystem;
interface IInteractable{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform source;
    public float range;
    public GameObject interactText;
    private IInteractable currentTarget;
    private void Update()
    {
        Ray r = new Ray(source.position, source.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, range))
        {
            if (hitInfo.collider.TryGetComponent(out IInteractable interactObj))
            {
                currentTarget = interactObj;
                interactText.SetActive(true);
                return;
            }
        }

        currentTarget = null;
        interactText.SetActive(false);
    }

    public void OnInteract(InputValue value)
    {
        if (currentTarget != null)
        {
            currentTarget.Interact();
        }
    }
}
