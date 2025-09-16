using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject endGamePanel;
    public Timer timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameController.gameOver)
        {
            if (timer) timer.StopAndWriteFinal(); // Timer já vai pausar
            if (endGamePanel) endGamePanel.SetActive(true);

            // opcional redundante (Timer já pausou), mas seguro:
            GameController.PauseGame();

            enabled = false;
        }
    }
}
