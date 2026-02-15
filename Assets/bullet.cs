 using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;

    public int rocketTime = 2;
    public int rocketDamage = 2;
    // private GameObject greenParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
    }


    private void OnTriggerEnter2D(Collider2D collision){
        Enemy enemy = collision.GetComponent<Enemy>();
        if( enemy != null ){

            enemy.TakeDamage(rocketDamage);

            this.rocketTime--;
            if( this.rocketTime == 0 ){
                Destroy(gameObject);
            }
        }
        // Instantiate(greenParticles, transform.position, transform.rotation );
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
