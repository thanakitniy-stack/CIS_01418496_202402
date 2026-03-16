using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // เพิ่ม Namespace สำหรับระบบใหม่

public class PauseMenu : MonoBehaviour
{
    public GameObject container; 
    public InputAction pauseAction; // สร้างตัวแปรรับค่า Action

    // เปิดใช้งาน Action เมื่อเริ่มเกม
    private void OnEnable() => pauseAction.Enable();
    private void OnDisable() => pauseAction.Disable();

    void Update()
    {
        // เช็กว่ามีการกดปุ่มที่ผูกไว้ (Escape) หรือไม่
        if (pauseAction.triggered)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (container != null)
        {
            bool isActive = !container.activeSelf;
            container.SetActive(isActive);
            Time.timeScale = isActive ? 0 : 1; // หยุด/เดินเวลา
        }
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("StartScene"); // ใส่ชื่อ Scene เมนูของคุณ
        Time.timeScale = 1;
    }
}