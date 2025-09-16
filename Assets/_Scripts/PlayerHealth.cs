using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
     [Header("Vida")]
    public int vidaMaxima = 3;
    public int vidaAtual;

    [Header("Invencibilidade pós-dano")]
    public float tempoDeInvencibilidade = 0.6f;

    [Header("UI (opcional)")]
    public GameObject endGamePanel;   // arraste do Canvas, se quiser mostrar fim de jogo
    public Timer timer;               // arraste o Timer da cena (para parar/escrever tempo)

    Rigidbody2D rb;
    PlayerMovement movement;
    bool invencivel;

    void Awake()
    {
        vidaAtual = vidaMaxima;
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();

        if (timer == null) timer = FindObjectOfType<Timer>(true);
        if (endGamePanel == null)
        {
            // Se seu UIManager expõe o painel publicamente, pegamos por aqui
            var ui = FindObjectOfType<UIManager>(true);
            if (ui != null) endGamePanel = ui.endGamePanel;
        }
    }

    public void TomarDano(int dano, Vector2 origem, float forcaEmpurrao, float tempoAtordoado = 0.2f)
    {
        if (invencivel || vidaAtual <= 0) return;

        vidaAtual -= Mathf.Max(1, dano);

        // Knockback
        Vector2 dir = ((Vector2)transform.position - origem).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * forcaEmpurrao, ForceMode2D.Impulse);

        // “Stun” rápido para o input não cancelar o empurrão
        if (movement != null) StartCoroutine(DesabilitarMovimento(tempoAtordoado));

        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        StartCoroutine(JanelaInvencibilidade());
    }

    System.Collections.IEnumerator DesabilitarMovimento(float t)
    {
        if (movement != null) movement.enabled = false;
        yield return new WaitForSeconds(t);
        if (movement != null) movement.enabled = true;
    }

    System.Collections.IEnumerator JanelaInvencibilidade()
    {
        invencivel = true;
        yield return new WaitForSeconds(tempoDeInvencibilidade);
        invencivel = false;
    }

    void Morrer()
    {
        if (movement != null) movement.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        if (timer != null) timer.StopAndWriteFinal();
        if (endGamePanel != null) endGamePanel.SetActive(true);
        // Se preferir outra lógica (reiniciar cena, voltar ao menu), dá pra colocar aqui.
    }
}