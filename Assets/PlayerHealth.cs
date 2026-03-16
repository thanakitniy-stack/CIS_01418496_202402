using System; // เพิ่มเข้ามาเพื่อให้ใช้ Action ได้
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;
    private SpriteRenderer spriteRenderer;

    // สร้าง Event เพื่อแจ้งเตือนเมื่อผู้เล่นตาย
    public static event Action OnPlayerDied;

    // void Start()
    // {
    //     currentHealth = maxHealth;
    //     healthUI.SetMaxHearts(maxHealth);
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    // }
    void Start()
    {
        ResetHealth(); // ตั้งค่าเลือดใหม่เมื่อเริ่มเกม
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        //GameController.OnReset += ResetHealth; // เชื่อมกับระบบรีเซ็ตเกม
        HealthItem.OnHealthCollect += Heal; // เมื่อมีการเก็บไอเทมเพิ่มเลือด ให้ไปเรียกฟังก์ชัน Heal
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    // ฟังก์ชันสำหรับการเพิ่มเลือด
    void Heal(int amount)
    {
        currentHealth += amount; // เพิ่มเลือดตามจำนวนที่กำหนด
        
        // ตรวจสอบไม่ให้เลือดเกินค่าสูงสุด
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthUI.UpdateHearts(currentHealth); // อัปเดตการแสดงผลบน UI
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // เปลี่ยน collision.GetComponent เป็น collision.gameObject.GetComponent
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        
        if (enemy != null)
        {
            TakeDamage(enemy.damage); // ลดเลือดตามค่า damage
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ตรวจสอบว่าสิ่งที่ชนมี Component ที่ใช้ Interface IItem หรือไม่
        IItem item = collision.GetComponent<IItem>();
        
        if (item != null)
        {
            item.Collect(); // เรียกใช้งานการเก็บไอเทม (ซึ่งจะไปเรียก OnHealthCollect ใน HealthItem)
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        StartCoroutine(FlashRed()); // แสดงเอฟเฟกต์ตัวแดง

        if (currentHealth <= 0)
        {
            // เมื่อเลือดหมด ให้ส่งสัญญาณว่าผู้เล่นตายแล้ว
            OnPlayerDied?.Invoke();
        }
    }
    void Update()
    {
        // ตรวจสอบตำแหน่งของผู้เล่นในทุกๆ เฟรม
        if (transform.position.y < -10f)
        {
            // ถ้าเลือดปัจจุบันยังมากกว่า 0 ให้ลดเลือดจนตาย หรือเรียก Event ตายทันที
            if (currentHealth > 0)
            {
                currentHealth = 0; // ตั้งค่าเลือดเป็น 0
                healthUI.UpdateHearts(currentHealth); // อัปเดต UI เลือด
                OnPlayerDied?.Invoke(); // ส่งสัญญาณว่าผู้เล่นตายแล้ว
            }
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}