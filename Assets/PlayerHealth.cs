using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement; // เพิ่มตรงนี้

    public static event Action OnPlayerDied;

    void Start()
    {
        ResetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>(); // เพิ่มตรงนี้

        HealthItem.OnHealthCollect += Heal;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthUI.UpdateHearts(currentHealth);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            TakeDamage(enemy.damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IItem item = collision.GetComponent<IItem>();
        if (item != null)
        {
            item.Collect();
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            playerMovement?.OnDeath(); // เสียงตาย
            OnPlayerDied?.Invoke();
        }
        else
        {
            playerMovement?.OnHit(); // เสียงโดนดาเมจ
        }
    }

    void Update()
    {
        if (transform.position.y < -10f)
        {
            if (currentHealth > 0)
            {
                currentHealth = 0;
                healthUI.UpdateHearts(currentHealth);
                playerMovement?.OnDeath(); // เสียงตายตอนตกหลุม
                OnPlayerDied?.Invoke();
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