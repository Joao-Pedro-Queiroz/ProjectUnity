using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Config")]
    public int pontosPorColetavel = 10;

    [Header("UI")]
    public TextMeshProUGUI scoreHUDText;    // Texto da HUD durante o jogo
    public TextMeshProUGUI scoreFinalText;  // Texto no painel final

    int score;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        AtualizarHUD(); // mostra "Pontuação: 0" se a HUD estiver ativa
    }

    public void Resetar()
    {
        score = 0;
        // Reativa HUD e limpa texto final ao começar um jogo novo
        if (scoreHUDText)   scoreHUDText.gameObject.SetActive(true);
        if (scoreFinalText) scoreFinalText.text = string.Empty;

        AtualizarHUD();
    }

    public void AdicionarColetavel()
    {
        score += Mathf.Max(1, pontosPorColetavel);
        AtualizarHUD();
    }

    void AtualizarHUD()
    {
        if (scoreHUDText) scoreHUDText.text = $"Pontuação: {score}";
    }

    public void EscreverScoreFinal()
    {
        // Esconde a HUD e escreve apenas a pontuação final
        if (scoreHUDText)   scoreHUDText.gameObject.SetActive(false);
        if (scoreFinalText) scoreFinalText.text = $"Pontuação Final: {score}";
    }

    public int GetScore() => score;
}