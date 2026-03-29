using UnityEngine;

public class GunSwitch : MonoBehaviour
{
    public SpriteRenderer gunSprite;

    public Sprite gun1;
    public Sprite gun2;
    public Sprite gun3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gunSprite.sprite = gun1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gunSprite.sprite = gun2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gunSprite.sprite = gun3;
        }
    }
}