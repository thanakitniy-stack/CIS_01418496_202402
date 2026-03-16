using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // ลาก Panel หน้าจอ Game Over มาใส่ในช่องนี้

    private void OnEnable()
    {
        // เริ่มดักฟังเหตุการณ์เมื่อผู้เล่นตาย
        PlayerHealth.OnPlayerDied += ShowGameOver;
    }

    private void OnDisable()
    {
        // เลิกดักฟังเมื่อ Object นี้ถูกทำลาย เพื่อป้องกัน Error
        PlayerHealth.OnPlayerDied -= ShowGameOver;
    }

    void ShowGameOver()
    {
        gameOverUI.SetActive(true); // แสดงหน้าจอ Game Over
        Time.timeScale = 0; // หยุดเวลาในเกม
    }
}