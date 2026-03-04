using UnityEngine;
using UnityEngine.InputSystem;

public class Cannon : MonoBehaviour
{

    public float damage = 10f;
    public float range = 100f;
    public Camera cam;
    public ParticleSystem muzzleFlash;
    public GameObject impact;
    public Player player;
    

    public float firerate = 2f;
    private float firenext = 0f;
    private bool isFiring = false;
    public float healthCost = 5f;

    // Update is called once per frame
    void Update()
    {
        if (isFiring && Time.time >= firenext) 
        {
            firenext = Time.time + 1f / firerate; Shoot(); 
        }
    }

    public void OnAttack(InputValue value)
    {
        isFiring = value.isPressed;
    }

    void Shoot()
    {
        player.TakeDamage(healthCost);
        muzzleFlash.Play();
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            GameObject impactobj = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactobj, 2f);
        }
    }
}
