using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(bulletDamage); // ส่งค่า damage ไปยัง Enemy
            Destroy(gameObject);
        }
    }
}