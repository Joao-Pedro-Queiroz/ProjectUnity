using UnityEngine;

public static class GameController
{
    private static int collectableCount;
    public static bool gameOver => collectableCount <= 0;

    // NOVO: controle de pause
    public static bool paused { get; private set; }

    public static void PauseGame()
    {
        if (paused) return;
        Time.timeScale = 0f;
        paused = true;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1f;
        paused = false;
    }

    public static void Init()
    {
        collectableCount = 4; // (ou conte dinamicamente as moedas)
        ResumeGame();         // garante que a cena comece “despausada”
        ScoreManager.Instance?.Resetar();  // ← zera o placar no começo
    }

    public static void Collect()
    {
        collectableCount--;
        ScoreManager.Instance?.AdicionarColetavel();
    }
}