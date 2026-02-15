using UnityEngine;

public class move : MonoBehaviour
{
    public float speed = 10f; // ตั้งค่าความเร็วใน Inspector ได้
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // เช็กว่าตัวกระสุนเองหันซ้ายหรือขวา (ดูจาก Scale X)
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        // สั่งให้วิ่งไปตามทิศทางนั้น
        rb.linearVelocity = new Vector2(direction * speed, 0);
        
        // ทำลายกระสุนอัตโนมัติเมื่อผ่านไป 2 วินาที (ป้องกันเกมหน่วง)
        Destroy(gameObject, 2f);
    }
}