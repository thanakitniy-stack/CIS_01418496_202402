using UnityEngine;
using UnityEngine.UI; // ต้องมีตัวนี้เพื่อจัดการปุ่ม

public class LevelButton : MonoBehaviour
{
    public int level; // ใส่เลขด่านของปุ่มนี้ (เช่น ปุ่มด่าน 2 ใส่เลข 2)

    void Start()
    {
        Button btn = GetComponent<Button>();

        // ดึงค่าจาก PlayerPrefs มาเช็ก ถ้าค่าที่บันทึกไว้ยังไม่ถึงด่านนี้ ให้กดปุ่มไม่ได้
        if (PlayerPrefs.GetInt("LevelReached", 1) < level)
        {
            btn.interactable = false; // ปิดการใช้งานปุ่ม (ปุ่มจะกลายเป็นสีเทา)
        }
    }
}