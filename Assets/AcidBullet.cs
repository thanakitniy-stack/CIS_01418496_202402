using UnityEngine;

public class AcidBullet : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 4f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}