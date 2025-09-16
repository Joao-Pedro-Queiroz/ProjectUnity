using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Tracks")]
    public AudioClip menuClip;
    public AudioClip gameplayClip;
    public AudioClip gameOverClip; // opcional

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;

    [Header("Crossfade")]
    public float defaultFade = 1.5f;

    AudioSource a, b;
    AudioSource active, inactive;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        a = gameObject.AddComponent<AudioSource>();
        b = gameObject.AddComponent<AudioSource>();
        foreach (var src in new[] { a, b })
        {
            src.loop = true;
            src.playOnAwake = false;
            src.spatialBlend = 0f; // 2D
            src.volume = 0f;
        }
        active = a; inactive = b;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Escolhe a trilha inicial com base na cena atual
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip target = (scene.buildIndex == 0) ? menuClip : gameplayClip;
        if (target != null && active.clip != target)
            FadeTo(target, defaultFade);
    }

    public void FadeTo(AudioClip clip, float duration)
    {
        if (clip == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(clip, duration));
    }

    System.Collections.IEnumerator FadeRoutine(AudioClip clip, float duration)
    {
        // Prepara a “faixa” inativa
        inactive.clip = clip;
        inactive.volume = 0f;
        inactive.Play();

        float t = 0f;
        float startVol = active.volume;
        // Usa unscaledDeltaTime para funcionar mesmo em timeScale = 0 (pausado)
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            inactive.volume = Mathf.Lerp(0f, musicVolume, k);
            active.volume   = Mathf.Lerp(startVol, 0f, k);
            yield return null;
        }

        // Troca as fontes
        active.Stop();
        active.volume = 0f;
        var tmp = active; active = inactive; inactive = tmp;
        active.volume = musicVolume;
    }

    // Opcional: chame no fim de jogo
    public void PlayGameOver(float fade = 0.8f)
    {
        if (gameOverClip != null) FadeTo(gameOverClip, fade);
    }

    public void SetVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        active.volume = musicVolume;
    }
}