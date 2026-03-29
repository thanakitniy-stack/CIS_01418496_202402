using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 10f;
    
    [Header("Movement Settings")]
    private Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;
    public int damage = 1;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("Audio")]
    public AudioClip aggroSound;   // เสียงร้องตอนเห็นผู้เล่น
    public AudioClip hitSound;     // เสียงโดนตี
    public AudioClip deathSound;   // เสียงตาย
    public float aggroSoundInterval = 4f; // ร้องทุกกี่วินาที

    private AudioSource audioSource;
    private bool isChasing = false;
    private float aggroSoundTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        ogColor = spriteRenderer.color;

        // สร้าง AudioSource อัตโนมัติ
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 0 = 2D, 1 = 3D
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 1.5f, groundLayer);
            if (groundInFront.collider != null)
                shouldJump = true;

            // เสียงร้องตอนไล่ตาม (ร้องซ้ำทุก aggroSoundInterval วินาที)
            if (!isChasing)
            {
                isChasing = true;
                PlaySFX(aggroSound); // ร้องครั้งแรกทันทีที่เห็นผู้เล่น
                aggroSoundTimer = aggroSoundInterval;
            }
            else
            {
                aggroSoundTimer -= Time.deltaTime;
                if (aggroSoundTimer <= 0f)
                {
                    PlaySFX(aggroSound);
                    aggroSoundTimer = aggroSoundInterval;
                }
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            isChasing = false; // รีเซ็ตเมื่อออกนอกระยะ
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PlaySFX(hitSound); // เสียงโดนตี
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
        // เล่นเสียงตายก่อน Destroy
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        foreach (LootItem lootItem in lootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
                InstantiateLoot(lootItem.itemPrefab);
            break;
        }

        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject loot)
    {
        if (loot)
            Instantiate(loot, transform.position, Quaternion.identity);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}