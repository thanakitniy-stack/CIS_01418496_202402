using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("lvl1"); // โหลดฉากเกม
    }

    public void OnExitClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // ปิดใน Editor
        #endif
            Application.Quit(); // ปิดเกมจริง
    }
}