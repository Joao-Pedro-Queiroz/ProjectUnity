using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;

    [Header("Ataque por contato")]
    public int dano = 1;
    public float forcaEmpurrao = 8f;
    public float tempoAtordoado = 0.15f;
    public float tempoEntreGolpes = 0.7f; // cooldown pra não dar dano a cada frame

    [Header("Áudio")]
    public AudioClip socoClip;
    AudioSource audioSrc;

    Transform alvo;          // Player
    Rigidbody2D rb;
    float tProximoGolpe;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null) audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
    }

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) alvo = player.transform;
    }

    void FixedUpdate()
    {
        if (alvo == null) return;
        Vector2 dir = (alvo.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * velocidade * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D col) { TentarGolpear(col); }
    void OnCollisionStay2D(Collision2D col)  { TentarGolpear(col); }

    void TentarGolpear(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;
        if (Time.time < tProximoGolpe) return;

        tProximoGolpe = Time.time + tempoEntreGolpes;

        var vida = col.collider.GetComponent<PlayerHealth>();
        if (vida != null)
        {
            vida.TomarDano(dano, transform.position, forcaEmpurrao, tempoAtordoado);
        }

        if (socoClip != null) audioSrc.PlayOneShot(socoClip);
    }
}
