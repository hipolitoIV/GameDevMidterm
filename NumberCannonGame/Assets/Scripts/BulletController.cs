using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 15f;
    public int bulletValue;
    
    // Call this from PlayerController after instantiating
    public void Initialize(int value)
    {
        bulletValue = value;
        
        // Give it velocity
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;

        // Destroy bullet after 5 seconds if it hits nothing
        Destroy(gameObject, 5f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit a crate
        if (collision.gameObject.CompareTag("Crate"))
        {
            CrateController crate = collision.gameObject.GetComponent<CrateController>();
            if (crate != null)
            {
                crate.TakeDamage(bulletValue);
            }
        }

        // Destroy the bullet on any collision
        Destroy(gameObject);
    }
}