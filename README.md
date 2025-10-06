# Swift Collect
### Jogo 2D (Unity) – Coletáveis, Inimigos, Timer e HUD

Projeto Unity 2D top-down onde o jogador coleta moedas dentro de um limite de tempo enquanto desvia de inimigos que perseguem, causam dano e empurram (knockback). O jogo exibe **pontuação**, **tempo**, **vida em corações** e **trilha sonora** contínua entre cenas. Ao fim, mostra um painel com o **tempo final** e a **pontuação final**, pausando o jogo.

## Sumário
- [Requisitos](#requisitos)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Fluxo do Jogo](#fluxo-do-jogo)
- [Cenas](#cenas)
- [Tags e Sorting Layers](#tags-e-sorting-layers)
- [Objetos e Configuração](#objetos-e-configuração)
- [Scripts (responsabilidades)](#scripts-responsabilidades)
- [Áudio](#áudio)
- [Controles](#controles)
- [Suporte a Controle (Gamepad)](#suporte-a-controle-gamepad)
- [Como Rodar](#como-rodar)
- [Dicas e Solução de Problemas](#dicas-e-solução-de-problemas)
- [Próximos Passos (idéias)](#próximos-passos-idéias)
- [Referências e créditos](#referências-e-créditos)

---

## Requisitos
- **Unity**: 2023.3+ / Unity 6 (testado com API Updater ativo; em 2D `Rigidbody2D.velocity` foi atualizado para `linearVelocity`).
- **TextMeshPro** (padrão do Unity).
- **Input Manager clássico** (eixos `Horizontal`/`Vertical`).

---

## Estrutura do Projeto
```
Assets/
  Scenes/
    MainMenu.unity
    SampleScene.unity
  _Scripts/
    GameController.cs
    MenuActions.cs
    PlayerMovement.cs
    PlayerHealth.cs
    EnemyAI.cs
    Timer.cs
    UIManager.cs
    ScoreManager.cs
    HealthHUD.cs     // HUD de corações (topo central)
    MusicManager.cs  // trilha sonora com crossfade e persistência
  Audio/
    urban-urban-city-street-music-277516.mp3
    punch-2-37333.mp3
    collect-points-190037.mp2
  Prefabs/
    Player
    Coin
    Enemy
```

---

## Fluxo do Jogo
- **Main Menu** → botão “Jogar” chama `GameController.Init()` e carrega a cena de jogo.
- **Gameplay (SampleScene)**:
  - **Timer** inicia (se `autoStart`).
  - **Player** se move e coleta moedas (+10 pontos por moeda).
  - **Inimigos** perseguem e, no contato, causam **dano** + **knockback** e tocam **som de soco**.
  - **HUD** mostra **Tempo**, **Pontuação**, **Vida (corações)**.
- **Fim de Jogo**:
  - Se o **tempo zera**, **todas as moedas** são coletadas ou **vida do jogador** acaba, o painel final aparece com **Tempo Final** e **Pontuação Final** e o jogo é **pausado** (`Time.timeScale = 0`).

---

## Cenas
Em **File ▸ Build Settings**:
1. **`0` – MainMenu**
2. **`1` – SampleScene**

A navegação por índice depende dessa ordem.

---

## Tags e Sorting Layers
**Tags** (crie em *Project Settings ▸ Tags and Layers*):
- `Player`
- `Coletavel` (moedas)
- `Inimigo` (inimigos)

**Sorting Layers** (se usar *Screen Space – Camera*):
- `Background`
- `World`
- `UI`

> Dica: se o **Canvas** estiver em **Screen Space – Overlay**, a UI já fica sempre na frente.

---

## Objetos e Configuração

### Canvas (HUD)
- **Render Mode**: *Screen Space – Overlay* (recomendado).
- **Canvas Scaler**: *Scale With Screen Size* (ex.: 1920×1080).
- Elementos:
  - **TimerText (TMP)** – texto em tempo real.
  - **ScoreHUDText (TMP)** – “Pontuação: X”.
  - **HeartsText (TMP)** – corações (♥) topo central.
  - **EndGamePanel** – painel oculto por padrão, com:
    - **FinalTimeText (TMP)** – “Tempo Final: …”
    - **ScoreFinalText (TMP)** – “Pontuação Final: …”

### GameManager (objeto vazio na raiz da cena)
- Componentes:
  - `Timer` (arraste: `timerText`, `endGamePanel`, `finalTimeText`, `timerContainer`).
  - `ScoreManager` (arraste: `scoreHUDText`, `scoreFinalText`).
  - (Opcional) `MusicManager` só na **MainMenu** (ele persiste entre cenas).

### Player
- `Tag = Player`
- `Rigidbody2D` (Body Type: Dynamic)
- `Collider2D` (não *Is Trigger*)
- `AudioSource` (som de coleta opcional)
- Scripts:
  - `PlayerMovement`
  - `PlayerHealth` (arraste `Timer` do **GameManager** e, se quiser, `EndGamePanel`)

### Coin (Moeda)
- `Tag = Coletavel`
- `Collider2D` **Is Trigger = ON**
- Ao tocar o player: som de coleta (se tiver) + destrói objeto.

### Enemy
- `Tag = Inimigo` (opcional para organização)
- `Rigidbody2D` (Dynamic) + `CircleCollider2D` (não trigger)
- **Constraints**: *Freeze Rotation* (não girar nas colisões)
- `AudioSource` com `socoClip`
- Script `EnemyAI` (persegue `Player` por tag; dá dano + knockback com cooldown)

---

## Scripts (responsabilidades)

### `GameController.cs` (estático)
- Estado global: contagem de coletáveis, `gameOver`, `paused`.
- `Init()` reseta contagem e **ResumeGame()** (zera `timeScale` para 1).
- `PauseGame()/ResumeGame()` controlam `Time.timeScale`.
- `Collect()` decrementa contagem e, quando zera, sinaliza fim (lido pelo `UIManager`).

### `MenuActions.cs`
- Botões do menu:
  - **Jogar**: `ResumeGame()`, `Init()`, `LoadScene(1)`.
  - **Menu**: `ResumeGame()`, `LoadScene(0)`.

### `PlayerMovement.cs`
- Leitura de `Horizontal/Vertical` (Input clássico) e move via `Rigidbody2D.MovePosition`.
- `OnTriggerEnter2D` com tag `Coletavel`: toca som (se `AudioSource`), chama `GameController.Collect()` e destrói a moeda.

### `PlayerHealth.cs`
- Vida (`vidaMaxima`, `vidaAtual`), i-frames (`tempoDeInvencibilidade`), **knockback**, morte.
- Ao morrer: desativa movimento, para rigidbody, chama `timer.StopAndWriteFinal()` e exibe painel final.

### `EnemyAI.cs`
- Persegue `Player` (encontra por tag).
- `OnCollisionEnter/Stay2D` com `Player`: aplica dano + **AddForce (Impulse)** para empurrão, respeitando `tempoEntreGolpes`.
- Toca `socoClip` via `AudioSource.PlayOneShot`.

### `Timer.cs`
- `timeLimit`, `countDown`, `autoStart`.
- Atualiza HUD; quando `t >= timeLimit` chama `StopAndWriteFinal()`.
- `StopAndWriteFinal()`:
  - escreve **Tempo Final**,
  - chama `ScoreManager.Instance?.EscreverScoreFinal()`,
  - mostra painel,
  - **`GameController.PauseGame()`**.

### `UIManager.cs`
- Monitora `GameController.gameOver` (todas as moedas coletadas).
- Ao detectar: chama `timer.StopAndWriteFinal()` e se desabilita.
- (Timer já pausa o jogo e escreve o tempo/pontuação final.)

### `ScoreManager.cs`
- **+10** pontos por moeda (configurável).
- HUD: “**Pontuação: X**” durante o jogo; no final: “**Pontuação Final: X**”.
- `Resetar()` no início da partida (chamado em `GameController.Init()`).

### `HealthHUD.cs` (corações)
- Mostra corações **cheios/vazios** no topo central.
- Esconde automaticamente quando o **EndGamePanel** for exibido.
- *Dica*: se um coração “grudar” ao trocar de cor, mantenha o separador entre blocos no `Refresh()`.

### `MusicManager.cs` (trilha sonora)
- `DontDestroyOnLoad`, duas `AudioSource` alternadas com **crossfade**.
- Preencha **menuClip** e **gameplayClip** (pode ser o mesmo arquivo).
- `SetVolume(float)` para ajuste de volume via UI.
- (Opcional) `PlayGameOver()` para jingle de fim.

---

## Áudio
- **Trilha**: use **Spatial Blend = 0 (2D)**, `loop = true`.  
  Em Import Settings, para faixas longas, **Load Type = Streaming**.
- **SFX curtos** (soco, coleta): `PlayOneShot(clip)` no `AudioSource` do objeto.

---

## Controles
- **Teclado**: WASD ou Setas para mover.
- **Controle (Gamepad)**: **Analógico esquerdo** ou **D‑Pad** para mover.  
  > Usando o **Input Manager clássico**, os eixos `Horizontal` e `Vertical` já leem o joystick por padrão. Se não funcionar de primeira, abra **Edit ▸ Project Settings ▸ Input Manager** e confira as entradas “Horizontal”/“Vertical” com **Type = Joystick Axis** (X/Y).

---

## Suporte a Controle (Gamepad)
Funciona com o **Input Manager clássico** sem mudar código (o `PlayerMovement` lê `Input.GetAxis("Horizontal"/"Vertical")`).

**Se o analógico não mover:**
1. **Edit ▸ Project Settings ▸ Input Manager**  
   - `Horizontal` → **Type: Joystick Axis**, **Axis: X axis**  
   - `Vertical`   → **Type: Joystick Axis**, **Axis: Y axis** (não invertido)  
   - `Dead: 0.19` (ou ajuste conforme o controle)
2. Garanta que o **Player** usa `PlayerMovement` baseado nesses eixos.
3. Teste com um controle (Xbox/PlayStation/Generic) conectado por USB/Bluetooth.

> **Opcional (New Input System):** se preferir migrar, instale o pacote **Input System**, habilite “Both” em **Player Settings ▸ Active Input Handling**, adicione **PlayerInput** ao Player e mapeie `Move (Vector2)` para `<Gamepad>/leftStick` e `<Keyboard>/wasd`. (Não é necessário para o escopo atual.)

---

## Como Rodar
1. Abra o projeto no Unity.
2. Garanta que as cenas estão em **Build Settings** (0=MainMenu, 1=SampleScene).
3. **Play** na `MainMenu`.
4. Clique em **Jogar**.

---

## Dicas e Solução de Problemas
- **UI atrás das paredes**: use **Canvas = Screen Space – Overlay** ou Sorting Layer `UI` > `World`.
- **Jogo entra “pausado” ao abrir cena diretamente**: chame `GameController.ResumeGame()` (já é feito em `Init()` e nos botões do menu).
- **Inimigos/jogador não param no fim**: confirme que `Timer.StopAndWriteFinal()` está sendo chamado (tempo zerado e vitória) e que ele chama `GameController.PauseGame()`.
- **Corações “grudam”** ao alternar cheio/vazio: mantenha o separador entre blocos no `HealthHUD.Refresh()` (já corrigido).
- **Som muito alto**: ajuste `MusicManager.musicVolume` no Inspector ou via slider chamando `SetVolume()`.

---

## Próximos Passos (idéias)
- Barra de vida (Image **Filled**) animada.
- Power-ups (imortalidade curta, velocidade, magnet de moedas).
- Spawner por ondas e contador de inimigos restantes.
- Leaderboard local (PlayerPrefs) e tela de resultados.
- Pausa com menu (Resume/Restart/Menu) respeitando `timeScale`.

---

## Referências e créditos
Liste aqui as referências de assets (música, efeitos sonoros, sprites, fontes) usados no projeto. Exemplos:

- Música: **“Nome da Faixa”** — Autor (Licença: CC‑BY 4.0) — link
- SFX: **“Punch 01”** — Biblioteca/Autor (Licença) — link
- Fonte/Ícones: **Noto Sans Symbols 2** (SIL Open Font License) — link
- Sprites: **Pacote X** — Autor (Licença) — link