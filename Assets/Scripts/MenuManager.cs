using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OyunaBasla()
    {
        SceneManager.LoadScene("MainScene"); // Oyun sahnenin adını buraya yaz
    }

    public void OyundanCik()
    {
        Application.Quit();
    }
}