using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator anim;

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

    // 🔥 เพิ่มตรงนี้ (Gun)
    public Transform gun;
    private Quaternion gunStartRotation;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    void Start()
    {
        anim = GetComponent<Animator>();

        // 🔥 จำตำแหน่งเริ่มต้นปืน
        if (gun != null)
        {
            gunStartRotation = gun.localRotation;
        }
    }

    void Update()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);

        GroundCheck();
        Gravity(); 
        FlipCheck();

        anim.SetBool("isWalking", horizontalMovement != 0);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void FlipCheck()
    {
        if (horizontalMovement < 0 && isFacingRight)
        {
            Flip();
        }
        else if (horizontalMovement > 0 && !isFacingRight)
        {
            Flip();
        }
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
            // ยิงกระสุน
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            float direction = isFacingRight ? 1f : -1f;
            bulletRb.velocity = new Vector2(direction * bulletSpeed, 0);

            anim.SetTrigger("shoot");

            // 🔥 ยกปืน
            if (gun != null)
            {
                StopAllCoroutines();
                StartCoroutine(GunRecoil());
            }
        }
    }

    // 🔥 Animation ปืน (ยกขึ้นแล้วกลับ)
    IEnumerator GunRecoil()
    {
        // ยกปืนขึ้น
        gun.localRotation = Quaternion.Euler(0, 0, 20f);

        yield return new WaitForSeconds(0.1f);

        // กลับตำแหน่งเดิม
        gun.localRotation = gunStartRotation;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpsRemaining--;

                anim.SetTrigger("jump");
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
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);

        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        if (groundCheckPos != null) Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}