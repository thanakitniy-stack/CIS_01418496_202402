using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int livePoint = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage){

        Debug.Log(damage);
        

        this.livePoint = this.livePoint - damage ;
        Debug.Log("Live Point : ");
        Debug.Log(this.livePoint);
        if( this.livePoint <= 0 ){
            Destroy(gameObject);        
        }
    }

}
