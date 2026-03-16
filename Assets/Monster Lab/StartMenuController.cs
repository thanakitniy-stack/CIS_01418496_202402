using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public GameObject startMainMenu;
    public GameObject levelSelect;
    public void OnStartClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // โหลดฉากเกม
    }

    public void GoToLevelSelect()
    {
        startMainMenu.SetActive(false);
        levelSelect.SetActive(true);
    }
    public void OnExitClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // ปิดใน Editor
        #endif
            Application.Quit(); // ปิดเกมจริง
    }
}