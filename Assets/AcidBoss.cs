using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidBoss : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 12f;

    [Header("Movement")]
    public float chaseSpeed = 2f;

    [Header("Shoot Attack (สกิลหลัก)")]
    public GameObject acidBulletPrefab;
    public float shootCooldown = 2f;
    public float shootRange = 8f;
    public float bulletSpeed = 6f;
    private float shootTimer = 0f;

    [Header("Acid Spray (สกิลรอง)")]
    public float sprayCooldown = 6f;
    public float sprayRange = 3f;
    public int sprayDamage = 1;
    public float sprayKnockback = 5f;
    private float sprayTimer = 0f;

    [Header("Health")]
    public int maxHealth = 15;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip spraySound;
    public AudioClip hitSound;
    public AudioClip deathSound;

    private AudioSource audioSource;
    private Transform player;
    private Rigidbody2D rb;
    private bool isActing = false;

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

        float dist = Vector2.Distance(transform.position, player.position);
        shootTimer -= Time.deltaTime;
        sprayTimer -= Time.deltaTime;

        if (dist > detectionRange)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if (isActing)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        // Spray รอง — ระยะใกล้ priority สูง
        if (dist <= sprayRange && sprayTimer <= 0f)
        {
            StartCoroutine(SprayRoutine());
        }
        // Shoot หลัก — ระยะกลาง
        else if (dist <= shootRange && shootTimer <= 0f)
        {
            StartCoroutine(ShootRoutine());
        }
        // Chase
        else
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);
        FlipSprite(direction);
    }

    private IEnumerator ShootRoutine()
    {
        isActing = true;
        shootTimer = shootCooldown;
        rb.velocity = Vector2.zero;

        PlaySFX(shootSound);

        // ยิง 3 ลูกติดกัน
        for (int i = 0; i < 3; i++)
        {
            ShootBullet();
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.3f);
        isActing = false;
    }

    void ShootBullet()
    {
        if (acidBulletPrefab == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(acidBulletPrefab, transform.position, Quaternion.identity);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
            bulletRb.velocity = dir * bulletSpeed;

        // หมุน bullet ให้หันไปทิศที่ยิง
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private IEnumerator SprayRoutine()
    {
        isActing = true;
        sprayTimer = sprayCooldown;
        rb.velocity = Vector2.zero;

        PlaySFX(spraySound);

        // Squish ก่อนพ่น
        yield return StartCoroutine(Squish(1.3f, 0.7f, 0.15f));

        // เช็คว่าผู้เล่นยังอยู่ในระยะไหม
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= sprayRange + 1f)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(sprayDamage);

            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockDir = (player.position - transform.position).normalized;
                playerRb.velocity = Vector2.zero;
                playerRb.AddForce(new Vector2(knockDir.x * sprayKnockback, 3f), ForceMode2D.Impulse);
            }
        }

        yield return new WaitForSeconds(0.4f);
        isActing = false;
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
            if (health != null) health.TakeDamage(1);
        }

        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(bullet.bulletDamage);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sprayRange);
    }
}