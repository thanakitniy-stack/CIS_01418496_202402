using UnityEngine;

public class NEXT : MonoBehaviour
{
    public void LoadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("lvl1");
        Time.timeScale = 1;
    }
}