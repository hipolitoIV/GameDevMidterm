using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera cam;

    void Update()
    {
        // --- 1. Aiming ---
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 cannonScreenPos = cam.WorldToScreenPoint(transform.position);
        Vector2 direction = mousePos - new Vector2(cannonScreenPos.x, cannonScreenPos.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // rotate only Z axis

        // --- 2. Shooting ---
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Shoot(1);
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Shoot(2);
        }
        else if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Shoot(3);
        }
    }

    void Shoot(int value)
    {
        // Log the shoot event
        Debug.Log($"Shoot triggered with value {value} at time {Time.time:F2}");

        // Check ammo
        if (GameManager.instance == null || !GameManager.instance.HasAmmo(value))
        {
            Debug.LogWarning("Attempted to shoot but no ammo!");
            return;
        }

        // Consume ammo and fire
        GameManager.instance.UseBullet(value);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<BulletController>().Initialize(value);
    }
}
