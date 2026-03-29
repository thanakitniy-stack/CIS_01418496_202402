using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    private PlayerMovement playerMovement; // เพิ่มตรงนี้

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>(); // เพิ่มตรงนี้
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector2 mousePos = Pointer.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        
        Vector3 shootDirection = (mouseWorldPosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(shootDirection.x, shootDirection.y) * bulletSpeed;
        
        Destroy(bullet, 2f);

        playerMovement?.OnShoot(); // เพิ่มตรงนี้ — เรียกเสียงยิง
    }
}