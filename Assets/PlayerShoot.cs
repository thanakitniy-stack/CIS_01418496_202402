using UnityEngine;
using UnityEngine.InputSystem; // ต้องเพิ่มบรรทัดนี้

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    void Update()
    {
        // เปลี่ยนจาก Input.GetMouseButtonDown เป็นระบบใหม่
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // หาตำแหน่งเมาส์ในระบบใหม่
        Vector2 mousePos = Pointer.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        
        Vector3 shootDirection = (mouseWorldPosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(shootDirection.x, shootDirection.y) * bulletSpeed;
        
        Destroy(bullet, 2f); // ทำลายกระสุนตามรูปภาพล่าสุด
    }
}