using UnityEngine;

public class NEXT2 : MonoBehaviour
{
    public void LoadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("lvl2");
        Time.timeScale = 1;
    }
}