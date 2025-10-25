using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; // An empty GameObject at the cannon's nozzle

    void Update()
    {
        // 1. Cannon Rotation
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        transform.up = direction;

        // 2. Shooting
        if (Input.GetKeyDown(KeyCode.S))
        {
            Shoot(1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Shoot(2);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot(3);
        }
    }

    void Shoot(int value)
    {
        // Check with GameManager if we have ammo
        if (GameManager.instance == null || !GameManager.instance.HasAmmo(value))
        {
            // Optionally play an "empty clip" sound
            return;
        }

        // We have ammo, so use it and fire
        GameManager.instance.UseBullet(value);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<BulletController>().Initialize(value);
    }
}