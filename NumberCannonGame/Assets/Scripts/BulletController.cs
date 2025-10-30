using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 15f;
    public int bulletValue;
    
    // Called from PlayerController when instantiated
    public void Initialize(int value)
    {
        bulletValue = value;
        
        // 1. Get the Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();

        // 2. Give it velocity.
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
        else
        {
            Debug.LogError("Bullet Prefab is missing a 3D Rigidbody component!");
        }


        // Destroy bullet after 5 seconds if it hits nothing
        Destroy(gameObject, 5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        bool shouldDestroy = true;
        // Check if we hit a crate
        if (collision.gameObject.CompareTag("Crate"))
        {
            CrateController crate = collision.gameObject.GetComponent<CrateController>();
            if (crate != null)
            {
                shouldDestroy = crate.TakeDamage(bulletValue);
            }
        }

        // Destroy the bullet on any collision
        if(shouldDestroy)
        {
            Destroy(gameObject);
        }
    }
}
