using UnityEngine;
using TMPro;

public class CrateController : MonoBehaviour
{
    public int crateValue;
    public TextMeshPro valueText;
    
    [Header("Bonus: Movement Variation")]
    public float fallSpeed = 2f;
    public float horizontalSpeed = 3f; // Speed of sine wave
    public float horizontalMagnitude = 0.5f; // Width of sine wave

    private Rigidbody rb;
    private float originalX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalX = transform.position.x;
        UpdateText();
    }

    // This is called by the CrateSpawner
    public void Initialize(int value, float speed)
    {
        crateValue = value;
        fallSpeed = speed;
        UpdateText();
    }

    void FixedUpdate()
    {
        // Basic falling
        Vector3 velocity = new Vector3(0, -fallSpeed, 0);

        // Bonus: Add sinusoidal (wobble) movement
        float xOffset = Mathf.Sin(Time.time * horizontalSpeed) * horizontalMagnitude;
        velocity.x = (originalX + xOffset - transform.position.x) / Time.fixedDeltaTime;

        rb.linearVelocity = velocity;
    }

    public void TakeDamage(int damage)
    {
        crateValue -= damage;
        UpdateText();

        if (crateValue == 0)
        {
            // Success
            GameManager.instance.HandleCrateDestruction(true, transform.position);
            Destroy(gameObject);
        }
        else if (crateValue < 0)
        {
            // Overshot
            GameManager.instance.HandleCrateDestruction(false, transform.position);
            Destroy(gameObject);
        }
        // If > 0, do nothing and let it keep falling
    }

    void UpdateText()
    {
        if (valueText != null)
        {
            valueText.text = crateValue.ToString();
        }
    }

    // This handles hitting the bottom of the screen
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BottomBoundary"))
        {
            // Penalty for missing
            GameManager.instance.HandleCrateDestruction(false, transform.position);
            Destroy(gameObject);
        }
    }
}