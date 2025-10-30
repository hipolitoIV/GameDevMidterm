using UnityEngine;
using TMPro;

public class CrateController : MonoBehaviour
{
    public int crateValue;
    public TextMeshPro valueText;

    [Header("Bonus: Movement Variation")]
    public float fallSpeed = 2f;
    public float horizontalSpeed = 3f;
    public float horizontalMagnitude = 0.5f;

    private Rigidbody rb;
    private float originalX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalX = transform.position.x;
        UpdateText();
    }

    public void Initialize(int value, float speed)
    {
        crateValue = value;
        fallSpeed = speed;
        UpdateText();
    }

    void FixedUpdate()
    {
        Vector3 velocity = new Vector3(0, -fallSpeed, 0);
        float xOffset = Mathf.Sin(Time.time * horizontalSpeed) * horizontalMagnitude;
        velocity.x = (originalX + xOffset - transform.position.x) / Time.fixedDeltaTime;
        rb.linearVelocity = velocity;
    }

    public bool TakeDamage(int damage)
    {
        if (damage > crateValue)
            return true; // Bullet destroyed, crate remains

        crateValue -= damage;
        crateValue = Mathf.Max(crateValue, 0);

        Debug.Log($"Crate hit! New value: {crateValue}");
        UpdateText();

        if (crateValue <= 0)
        {
            GameManager.instance.HandleCrateDestruction(true, transform.position);
            Destroy(gameObject);
        }

        return true;
    }

    void UpdateText()
    {
        if (valueText != null)
            valueText.text = crateValue.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BottomBoundary"))
        {
            Debug.Log("Crate hit bottom.");
            GameManager.instance.HandleCrateDestruction(false, transform.position);
            Destroy(gameObject);
        }
    }
}
