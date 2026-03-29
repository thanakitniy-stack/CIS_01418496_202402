using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HitTarget(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HitTarget(collision.gameObject);
    }

    private void HitTarget(GameObject target)
    {
        // ชน Enemy
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(bulletDamage);
            Destroy(gameObject);
            return;
        }

        // ชน SlimeBoss
        SlimeBoss boss = target.GetComponent<SlimeBoss>();
        if (boss != null)
        {
            boss.TakeDamage(bulletDamage);
            Destroy(gameObject);
            return;
        }

        // ชน SlimeChild
        SlimeChild child = target.GetComponent<SlimeChild>();
        if (child != null)
        {
            child.TakeDamage(bulletDamage);
            Destroy(gameObject);
            return;
        }

        }
    }
