using UnityEngine;
using UnityEngine.SceneManagement; // เพิ่มเพื่อให้เรียกใช้ SceneManager ได้ง่ายขึ้น

public class RestartGame3 : MonoBehaviour
{
    public void LoadCurrentScene()
    {
        // โหลดฉากที่ชื่อว่า "lvl3"
        SceneManager.LoadScene("lvl3");
        
        // คืนค่าความเร็วเกมเป็นปกติ (เผื่อกรณีที่เกมหยุดเวลาไว้ตอนตาย)
        Time.timeScale = 1;
    }
}