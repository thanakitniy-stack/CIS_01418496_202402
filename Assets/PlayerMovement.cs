using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;
    private bool isFacingRight = true; // เช็กว่าหันขวาอยู่ไหม

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("Shooting")]
    public GameObject bulletPrefab; // ลาก Prefab กระสุนมาใส่
    public Transform firePoint;     // ลาก Object FirePoint มาใส่
    public float bulletSpeed = 15f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    void Update()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
        GroundCheck();
        Gravity(); 
        FlipCheck(); // เช็กการหันหน้าทุกเฟรม
    }

    private void FlipCheck()
    {
        // ถ้ากดไปซ้ายแต่หน้าหันขวา หรือ กดไปขวาแต่หน้าหันซ้าย ให้ทำการ Flip
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
        localScale.x *= -1f; // สลับค่า X เป็นบวก/ลบ
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    // ฟังก์ชันสำหรับปุ่มยิง (สร้าง Action ใหม่ใน Input System ชื่อ Fire หรือใช้ปุ่มเดิม)
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // สร้างกระสุนที่ตำแหน่ง FirePoint
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            // ยิงกระสุนไปตามทิศที่ตัวละครหัน (ใช้ localScale.x หรือทิศทางของตัวละคร)
            float direction = isFacingRight ? 1f : -1f;
            bulletRb.velocity = new Vector2(direction * bulletSpeed, 0);
        }
    }

    // --- ส่วนที่เหลือ (Jump, Gravity, GroundCheck) คงเดิมตามโค้ดของคุณ ---
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpsRemaining--;
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
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        if(groundCheckPos != null) Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}