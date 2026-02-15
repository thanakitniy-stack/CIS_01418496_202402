using UnityEngine;
using UnityEngine.InputSystem;

public class Piwpiw : MonoBehaviour
{
    public Transform piwpiwShootingPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f; // ตั้งความเร็วกระสุน

    void Update()
    {
        // ใช้ Shift ในการยิง
        if(Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    // ในฟังก์ชัน Shoot() ของสคริปต์ Piwpiw
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, piwpiwShootingPoint.position, transform.rotation);
        
        // ส่งค่าทิศทางไปให้กระสุน (เช็กจาก scale ของคนยิง)
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        
        // ปรับ Scale ของกระสุนให้หันตามตัวละคร
        Vector3 bulletScale = bullet.transform.localScale;
        bulletScale.x = Mathf.Abs(bulletScale.x) * direction;
        bullet.transform.localScale = bulletScale;
    }
}