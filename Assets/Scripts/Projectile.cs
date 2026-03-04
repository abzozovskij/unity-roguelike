using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.collider.GetComponentInParent<Player>();
        Debug.Log("Hit: " + collision.collider.name);
        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
