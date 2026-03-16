using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 10f; // ระยะที่ศัตรูจะมองเห็นและเริ่มเดินมาหา
    
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

    //LootTable
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // ตรวจสอบว่าในฉากมี Object ที่ใส่ Tag ว่า "Player"
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        ogColor = spriteRenderer.color;
    }

    void Update()
    {
        if (player == null) return;

        // 1. คำนวณระยะห่างระหว่าง ศัตรู กับ ผู้เล่น
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 2. ถ้าผู้เล่นอยู่ในระยะที่กำหนด (detectionRange) ถึงจะเริ่มเดิน
        if (distanceToPlayer <= detectionRange)
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            
            // สั่งให้เดินตาม
            rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

            // ระบบ AI ตรวจสอบการกระโดด (กำแพง/ทางขาด) ตามรูปภาพที่คุณเคยส่งมา
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 1.5f, groundLayer);
            if (groundInFront.collider != null) 
            {
                shouldJump = true; 
            }
        }
        else
        {
            // ถ้าอยู่นอกระยะ ให้หยุดเดิน (หรือจะใส่ระบบเดินตรวจการณ์เพิ่มก็ได้)
            rb.velocity = new Vector2(0, rb.velocity.y);
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
        // วนลูปตรวจสอบรายการไอเทมใน lootTable
        foreach(LootItem lootItem in lootTable) 
        {
            // สุ่มตัวเลข 0-100 ถ้าผลลัพธ์น้อยกว่าหรือเท่ากับโอกาสดรอป ให้ทำการสร้างไอเทม
            if(Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemPrefab);
            }
            break;
        }

        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject loot)
    {
        if(loot)
        {
            // สร้างไอเทมขึ้นมาที่ตำแหน่งปัจจุบันของศัตรู
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);

            // เปลี่ยนสีของไอเทมที่ดรอปให้เป็นสีแดง
            //droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    // วาดวงกลมในหน้า Scene เพื่อให้เราเห็นระยะการมองเห็นของศัตรู
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}