using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Config")]
    public float timeLimit = 60f;      // limite (segundos)
    public bool countDown = true;      // se quiser regressivo
    public bool autoStart = true;      

    [Header("Referências (arraste no Inspector)")]
    public TextMeshProUGUI timerText;  // TimerText
    public GameObject endGamePanel;    // EndGamePanel
    public TextMeshProUGUI finalTimeText; // FinalTimeText
    public GameObject timerContainer; 

    [Header("Eventos")]
    public UnityEvent onTimeUp;        // dá pra ligar coisas pelo Inspector

    float t;
    bool running;

    void Start() { running = autoStart; t = 0f; UpdateHUD(); }

    public void StartTimer() { running = true; }
    public void StopTimer()  { running = false; }

    void Update()
    {
        if (!running) return;
        t += Time.deltaTime;

        // condição de término pelo tempo
        if (t >= timeLimit)
        {
            running = false;
            t = timeLimit;           // garante que o HUD mostre 00:00.00 no modo regressivo
            onTimeUp?.Invoke();      // dispare eventos antes de pausar (se precisar)
            StopAndWriteFinal();     // mostra painel, escreve "Tempo Final: 00:00.00" e PAUSA o jogo
            return;                  // sai do Update para evitar HUD extra
        }
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (!timerText) return;
        float show = countDown ? Mathf.Max(0f, timeLimit - t) : t;
        timerText.text = $"Tempo: {Format(show)}";
    }

    string Format(float sec)
    {
        int m = Mathf.FloorToInt(sec / 60f);
        int s = Mathf.FloorToInt(sec % 60f);
        int cs = Mathf.FloorToInt((sec - Mathf.Floor(sec)) * 100f); // centésimos
        return $"{m:00}:{s:00}.{cs:00}";
    }

    public float GetShownTime()
    {
        return countDown ? Mathf.Max(0f, timeLimit - t) : t;
    }

    public string GetShownTimeFormatted()
    {
        return $"Tempo Final: {Format(GetShownTime())}";
    }

    public void StopAndWriteFinal()
    {
        running = false;
        if (timerContainer) timerContainer.SetActive(false);
        if (finalTimeText)  finalTimeText.text = GetShownTimeFormatted();

        ScoreManager.Instance?.EscreverScoreFinal(); // ← escreve "Score Final: X"

        if (endGamePanel)   endGamePanel.SetActive(true);
        GameController.PauseGame();
    }

}