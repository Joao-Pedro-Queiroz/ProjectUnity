using UnityEngine;
using TMPro;

public class HealthHUD : MonoBehaviour
{
    [Header("Refs")]
    public PlayerHealth player;               // arraste o Player (ou deixa em branco)
    public TextMeshProUGUI heartsText;        // opcional: se vazio, pega o TMP do mesmo GO
    public GameObject endGamePanel;           // opcional: se vazio, tenta achar via UIManager
    public bool hideOnEnd = true;

    [Header("Aparência")]
    [Tooltip("Um único caractere: ♥, ❤, ● etc.")]
    public string heartChar = "♥";
    public string fullHex = "#FF3B3B";        // cor dos cheios
    public string emptyHex = "#777777";       // cor dos vazios
    public bool useSpacing = true;
    public string spacer = " ";               // espaço entre corações

    int lastHP = -1, lastMax = -1;

    void Awake()
    {
        if (!player) player = FindObjectOfType<PlayerHealth>(true);
        if (!heartsText) heartsText = GetComponent<TextMeshProUGUI>();
        if (!endGamePanel)
        {
            var ui = FindObjectOfType<UIManager>(true);
            if (ui) endGamePanel = ui.endGamePanel;
        }
        Refresh();
    }

    void Update()
    {
        if (!player) return;

        // Esconde no fim de jogo
        bool ended = hideOnEnd && endGamePanel && endGamePanel.activeInHierarchy;
        if (heartsText) heartsText.gameObject.SetActive(!ended);
        if (ended) return;

        if (player.vidaAtual != lastHP || player.vidaMaxima != lastMax)
            Refresh();
    }

    void Refresh()
    {
        lastHP  = Mathf.Clamp(player.vidaAtual, 0, player.vidaMaxima);
        lastMax = Mathf.Max(1, player.vidaMaxima);

        char c = string.IsNullOrEmpty(heartChar) ? '♥' : heartChar[0];
        string filled = new string(c, lastHP);
        string empty  = new string(c, lastMax - lastHP);

        string sep = useSpacing ? spacer : "";

        // prepara os blocos já com espaçamentos internos
        string filledPart = InsertSep(filled, sep);
        string emptyPart  = InsertSep(empty,  sep);

        // <<< AQUI ESTÁ O PULO DO GATO >>>
        // se há cheios e vazios, insira UM separador entre os blocos
        string between = (useSpacing && filled.Length > 0 && empty.Length > 0) ? sep : "";

        if (heartsText)
        {
            heartsText.text =
                $"<color={fullHex}>{filledPart}</color>{between}" +
                (empty.Length > 0 ? $"<color={emptyHex}>{emptyPart}</color>" : "");
        }
    }

    string InsertSep(string s, string sep)
    {
        if (string.IsNullOrEmpty(sep) || s.Length <= 1) return s;
        System.Text.StringBuilder sb = new System.Text.StringBuilder(s.Length * 2);
        for (int i = 0; i < s.Length; i++)
        {
            sb.Append(s[i]);
            if (i < s.Length - 1) sb.Append(sep);
        }
        return sb.ToString();
    }
}
