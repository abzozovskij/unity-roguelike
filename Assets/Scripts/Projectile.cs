using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public float damageScaling = 5f;
    public float lifeTime = 5f;

    public void SetEnemy(Enemy enemy)
    {
        if (enemy.level > 1)
        {
            damage += damageScaling * (enemy.level - 1);
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player != null)
        {
            player.TakeDamage(damage, false);
        }
        Destroy(gameObject);
    }

}
