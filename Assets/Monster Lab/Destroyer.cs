using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 12f;

    [Header("Movement")]
    public float chaseSpeed = 3f;
    public float jumpForce = 14f;

    [Header("Ground Slam")]
    public float slamDetectionRadius = 4f;
    public float slamKnockbackForce = 12f;
    public float slamKnockbackUpForce = 6f;
    public float slamDamage = 2;
    public float slamCooldown = 4f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Melee Attack")]
    public float meleeRange = 1.5f;
    public int meleeDamage = 1;
    public float meleeCooldown = 1.2f;

    [Header("Health")]
    public int maxHealth = 20;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip slamSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip meleeSound;

    private AudioSource audioSource;
    private Transform player;
    private Rigidbody2D rb;

    private bool isGrounded;
    private bool isSlamming = false;
    private bool isJumping = false;

    private float slamTimer = 0f;
    private float meleeTimer = 0f;

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

    CheckGrounded();

    float dist = Vector2.Distance(transform.position, player.position);
    slamTimer -= Time.deltaTime;
    meleeTimer -= Time.deltaTime;

    if (dist > detectionRange)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        return;
    }

    if (isSlamming || isJumping) return;

    Debug.Log($"dist={dist:F1} | meleeRange={meleeRange} | slamRadius={slamDetectionRadius} | slamTimer={slamTimer:F1} | isGrounded={isGrounded}");

    if (dist <= meleeRange)
    {
        if (meleeTimer <= 0f)
        {
            MeleeAttack();
        }
    }
    else if (dist <= slamDetectionRadius && slamTimer <= 0f && isGrounded)
    {
        Debug.Log(">>> เข้า Slam!");
        StartCoroutine(SlamRoutine());
    }
    else
    {
        Debug.Log($"Chase | slamTimer={slamTimer:F1} | isGrounded={isGrounded}");
        ChasePlayer();
    }
}

    void CheckGrounded()
    {
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        else
            isGrounded = Mathf.Abs(rb.velocity.y) < 0.1f;
    }

    void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);
        FlipSprite(direction);
    }

    void MeleeAttack()
    {
        meleeTimer = meleeCooldown;
        PlaySFX(meleeSound);
        StartCoroutine(MeleeRoutine());
    }

    private IEnumerator MeleeRoutine()
    {
        rb.velocity = Vector2.zero;

        float dir = Mathf.Sign(player.position.x - transform.position.x);

        // พุ่งนิดนึง
        rb.AddForce(new Vector2(dir * 4f, 0), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);

        // 👉 จุดโจมตี (เลื่อนไปด้านหน้า)
        Vector2 attackPoint = (Vector2)transform.position + new Vector2(dir * 1f, 0);

        // 👉 ตรวจจับทุกตัว
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint, meleeRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Melee hit!");

                PlayerHealth health = hit.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(meleeDamage);
                }

                Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                    playerRb.velocity = Vector2.zero;
                    playerRb.AddForce(new Vector2(knockDir.x * 4f, 2f), ForceMode2D.Impulse);
                }
            }
        }
    }

   private IEnumerator SlamRoutine()
{
    isSlamming = true;
    rb.velocity = Vector2.zero;

    isJumping = true;
    PlaySFX(jumpSound);

    yield return StartCoroutine(Squish(0.6f, 1.4f, 0.15f));

    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    slamTimer = slamCooldown;

    yield return new WaitUntil(() => rb.velocity.y <= 0f);
    isJumping = false;

    rb.gravityScale = 3f;

    // เพิ่ม timeout กันค้าง
    float timeout = 3f;
    while (!isGrounded && timeout > 0f)
    {
        timeout -= Time.deltaTime;
        Debug.Log($"รอลงพื้น... isGrounded={isGrounded} | groundCheck={groundCheck} | velocity={rb.velocity.y:F2}");
        yield return null;
    }

    rb.gravityScale = 1f;

    Debug.Log("แตะพื้นแล้ว! Slam!");
    PlaySFX(slamSound);
    yield return StartCoroutine(Squish(1.5f, 0.5f, 0.1f));

    // Slam damage
    float dist = Vector2.Distance(transform.position, player.position);
    if (dist <= slamDetectionRadius)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null) health.TakeDamage((int)slamDamage);

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockDir = (player.position - transform.position).normalized;
            playerRb.velocity = Vector2.zero;
            playerRb.AddForce(
                new Vector2(knockDir.x * slamKnockbackForce, slamKnockbackUpForce),
                ForceMode2D.Impulse
            );
        }
    }

    yield return new WaitForSeconds(0.3f);
    isSlamming = false;
}

    private IEnumerator Squish(float scaleX, float scaleY, float duration)
    {
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
        yield return new WaitForSeconds(duration);
        transform.localScale = Vector3.one;
    }

    void FlipSprite(float direction)
    {
        if (direction != 0)
            spriteRenderer.flipX = direction < 0;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        PlaySFX(hitSound);
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

        foreach (LootItem lootItem in lootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
                Instantiate(lootItem.itemPrefab, transform.position, Quaternion.identity);
            break;
        }

        Destroy(gameObject);
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(meleeDamage);
            Debug.Log("Collision hit!");

            // Knockback
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockDir = (collision.transform.position - transform.position).normalized;
                playerRb.velocity = Vector2.zero;
                playerRb.AddForce(new Vector2(knockDir.x * 4f, 2f), ForceMode2D.Impulse);
            }
        }
    }
    

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        void OnDrawGizmos()
{
    if (groundCheck != null)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamDetectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}