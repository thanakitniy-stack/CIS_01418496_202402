using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 10f;

    [Header("Movement")]
    public float chaseSpeed = 2f;

    [Header("Spawn Settings")]
    public GameObject slimeChildPrefab;
    public int maxChildren = 5;
    public float spawnInterval = 3f;
    private float spawnTimer = 0f;
    private int currentChildren = 0;

    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;
    public int damage = 1;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("Audio")]
    public AudioClip spawnSound;
    public AudioClip hitSound;
    public AudioClip deathSound;

    private AudioSource audioSource;
    private Transform player;
    private Rigidbody2D rb;

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

        if (dist <= detectionRange)
        {
            // เดินตามผู้เล่น
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

            // ปล่อยลูก
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f && currentChildren < maxChildren)
            {
                SpawnChild();
                spawnTimer = spawnInterval;
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void SpawnChild()
    {
        if (slimeChildPrefab == null) return;

        // Spawn ข้างๆ แม่
        float side = Random.value > 0.5f ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(side * 1.2f, 0f, 0);
        GameObject child = Instantiate(slimeChildPrefab, spawnPos, Quaternion.identity);

        // ตั้ง Layer ให้ลูกเหมือนแม่
        child.layer = gameObject.layer;

        SlimeChild childScript = child.GetComponent<SlimeChild>();
        if (childScript != null)
        {
            childScript.mother = this;
            currentChildren++;

            // ให้ลูก ignore Collider ของแม่
            Collider2D motherCol = GetComponent<Collider2D>();
            Collider2D childCol = child.GetComponent<Collider2D>();
            if (motherCol != null && childCol != null)
                Physics2D.IgnoreCollision(childCol, motherCol);
        }

        PlaySFX(spawnSound);
        StartCoroutine(SpawnSquish());
    }

    public void OnChildDied()
    {
        currentChildren--;
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

    private IEnumerator SpawnSquish()
    {
        transform.localScale = new Vector3(1.3f, 0.7f, 1f);
        yield return new WaitForSeconds(0.1f);
        transform.localScale = Vector3.one;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}