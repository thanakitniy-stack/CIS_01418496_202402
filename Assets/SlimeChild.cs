using System.Collections;
using UnityEngine;

public class SlimeChild : MonoBehaviour
{
    [HideInInspector] public SlimeBoss mother;

    [Header("Movement")]
    public float jumpForceX = 5f;
    public float jumpForceY = 6f;
    public float jumpCooldown = 1f;
    public LayerMask groundLayer;

    [Header("Health")]
    public int maxHealth = 2;
    public int damage = 1;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip deathSound;

    private Rigidbody2D rb;
    private Transform player;
    private AudioSource audioSource;
    private float jumpTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        ogColor = spriteRenderer.color;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (player == null) return;

        // Raycast จากขอบล่าง Collider
        Collider2D col = GetComponent<Collider2D>();
        float bottomY = col != null ? col.bounds.min.y : transform.position.y;
        Vector2 rayOrigin = new Vector2(transform.position.x, bottomY + 0.05f);

        bool isGrounded = Physics2D.Raycast(rayOrigin, Vector2.down, 0.15f, groundLayer);
        Debug.DrawRay(rayOrigin, Vector2.down * 0.15f, isGrounded ? Color.green : Color.red);

        jumpTimer -= Time.deltaTime;

        if (isGrounded && jumpTimer <= 0f)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * jumpForceX, jumpForceY);
            PlaySFX(jumpSound);
            jumpTimer = jumpCooldown;
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        StartCoroutine(FlashWhite());
        if (currentHealth <= 0) Die();
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = ogColor;
    }

    void Die()
    {
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (mother != null)
            mother.OnChildDied();

        Destroy(gameObject);
    }

private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
        if (health != null) health.TakeDamage(damage);
    }

    // รับกระสุน
    Bullet bullet = collision.gameObject.GetComponent<Bullet>();
    if (bullet != null)
    {
        TakeDamage(bullet.bulletDamage);
        Destroy(collision.gameObject);
    }
}
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }
}