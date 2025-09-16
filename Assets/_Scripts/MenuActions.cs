using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public void IniciarJogo()
    {
        GameController.ResumeGame(); // garante timeScale = 1
        GameController.Init();
        SceneManager.LoadScene(1);
    }

    public void Menu()
    {
        GameController.ResumeGame(); // garante timeScale = 1
        SceneManager.LoadScene(0);
    }
}
