using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    Animator anim;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;
    private bool isFacingRight = true;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("Audio")]
    public AudioClip jumpSound;        // เสียงกระโดด
    public AudioClip shootSound;       // เสียงยิง
    public AudioClip deathSound;       // เสียงตาย
    public AudioClip hitSound;         // เสียงโดนดาเมจ
    public AudioClip bgMusic;          // เสียงพื้นหลัง

    private AudioSource sfxSource;     // สำหรับเสียง Effect
    private AudioSource bgmSource;     // สำหรับเสียงพื้นหลัง

    private bool isGrounded = false;
    private bool wasPreviouslyGrounded = false;

    void Awake()
    {
        // สร้าง AudioSource สองตัว: หนึ่งสำหรับ SFX หนึ่งสำหรับ BGM
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;           // BGM วนซ้ำตลอด
        bgmSource.volume = 0.5f;         // ลดเสียง BGM ให้ไม่กลบ SFX

        // เริ่มเล่น BGM ทันที
        if (bgMusic != null)
        {
            bgmSource.clip = bgMusic;
            bgmSource.Play();
        }
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
        GroundCheck();
        Gravity();
        FlipCheck();
        // 👇 คุม animation เดิน
        anim.SetBool("isMoving", Mathf.Abs(horizontalMovement) > 0.1f);
    }

    private void FlipCheck()
    {
        if (horizontalMovement < 0 && isFacingRight)
            Flip();
        else if (horizontalMovement > 0 && !isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            float direction = isFacingRight ? 1f : -1f;
            bulletRb.velocity = new Vector2(direction * bulletSpeed, 0);

            // เล่นเสียงยิง
            PlaySFX(shootSound);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpsRemaining--;

                // เล่นเสียงกระโดด
                PlaySFX(jumpSound);
            }
            else if (context.canceled)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
    }

    private void Gravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void GroundCheck()
    {
        wasPreviouslyGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);

        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }
    }

    // ============================================================
    // ฟังก์ชัน Public สำหรับเรียกจากสคริปต์อื่น (Health/Enemy)
    // ============================================================

    /// <summary>
    /// เรียกเมื่อตัวละครโดนดาเมจ
    /// </summary>
    public void OnHit()
    {
        PlaySFX(hitSound);
    }
    // เพิ่มใน PlayerMovement.cs ต่อจาก OnHit() และ OnDeath()
    public void OnShoot()
    {
    PlaySFX(shootSound);
    }
    /// <summary>
    /// เรียกเมื่อตัวละครตาย
    /// </summary>
    public void OnDeath()
    {
        PlaySFX(deathSound);

        // หยุด BGM ตอนตาย
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    /// <summary>
    /// เล่นเสียง BGM ใหม่ (เช่น ตอน Respawn หรือเปลี่ยนด่าน)
    /// </summary>
    public void PlayBGM(AudioClip clip = null)
    {
        bgmSource.clip = clip != null ? clip : bgMusic;
        bgmSource.Play();
    }

    // ============================================================
    // Helper
    // ============================================================
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        if (groundCheckPos != null)
            Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}