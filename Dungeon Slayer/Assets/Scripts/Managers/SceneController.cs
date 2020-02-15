using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class SceneController : MonoBehaviour {

    public static SceneController instance;

    public Animator fadeAnimator;
    [SerializeField] float fadeTime = 1f;
    public Text timeText;
    [SerializeField] private float timeToEscape = 300f;
    [SerializeField] private float heartCollapseTime = 6f;
    private float timeScore = 0f;
    private Coroutine collapse = null;
    private int playerScore = 0;

    // Essa funcao e chamada quando o objeto e instanciado pela primeira vez
    void Awake() {
        // So pode existir uma instancia dessa classe
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
        // Este objeto (SceneController) ira persistir por todas as cenas
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        AudioManager.instance.Play("MenuMusic");
        EventsManager.current.onHeartDestroy += EndGameWin; // Inscreve o metodo "EndGameWin" como uma das acoes a serem tomadas quando o coracao da Dungeon for destruido
        EventsManager.current.onPlayerDestroy += EndGameLoss;   // Inscreve o metodo "EndGameLoss" como uma das acoes a serem tomadas quando o jogador morrer ou o tempo acabar
    }

    public void GoToMainMenu() {
        StartCoroutine(ChangeSceneCor(0));
    }

    public void StartGame() {
        StartCoroutine(ChangeSceneCor(1));
    }

    public void ChangeScene(int index) {
        StartCoroutine(ChangeSceneCor(index));
    }

    // Corotina que vai para a cena passada por parametro
    private IEnumerator ChangeSceneCor(int index) {
        // Comeca a animacao de transicao (um fade preto) se for uma cena do jogo
        if (index > 0 && index < 4) {
            fadeAnimator.SetTrigger("Fade");
            // Espera um tempo para que a transicao ocorra
            yield return new WaitForSeconds(fadeTime);
        }
        int sceneBefore = SceneManager.GetActiveScene().buildIndex;
        // Carrega ela
        SceneManager.LoadScene(index);
        // Decide qual musica de fundo ira tocar
        switch (index) {
            case 0:
                if (sceneBefore != 5) { // Se a cena anterior nao eh a do highscore
                    AudioManager.instance.StopAll();
                    AudioManager.instance.Play("MenuMusic");   
                }
                break;
            case 1:
                AudioManager.instance.StopAll();
                AudioManager.instance.Play("EntranceMusic");
                break;
            case 2:
                AudioManager.instance.StopAll();
                AudioManager.instance.Play("BossBattleMusic");
                timeText.gameObject.SetActive(true);    // Ativa o texto que ira mostrar o tempo restante para sair da Dungeon
                DontDestroyOnLoad(timeText.transform.parent.gameObject); // Faz com que o texto nao seja destruido ao se trocar de cena
                collapse = StartCoroutine(Collapse(timeToEscape));
                break;
            case 3:
                AudioManager.instance.StopAll();
                AudioManager.instance.Play("DungeonHeartMusic");
                break;
            case 4:
                AudioManager.instance.StopAll();
                AudioManager.instance.Play("DungeonHeartMusic");
                timeText.gameObject.SetActive(false);
                break;
            case 5:
                AudioManager.instance.StopAll();
                AudioManager.instance.Play("MenuMusic");
                timeText.gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator Collapse(float initTime) {
        float curTime = initTime;
        string minutes, seconds;
        while (curTime > 0) {
            minutes = Mathf.Floor(curTime/60).ToString("00");
            seconds = Mathf.Floor(curTime%60).ToString("00");
            timeText.text = minutes + ":" + seconds;
            curTime -= Time.deltaTime;
            this.timeScore = initTime - curTime;
            yield return null;
        }
        EndGameLoss();
    }

    void EndGameWin() {
        // Toca a animacao do boss final morrendo
        fadeAnimator.SetTrigger("FadeWhite");
        StartCoroutine(EndGameWinCor());
    }

    void EndGameLoss() {
        // Para a corotina que conta o tempo
        StopCoroutine(collapse);
        // Acha o jogador e o destroi (junto da sua barra de vida)
        PlayerManager player = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
        player.DestroySelfAndHealthbar();
        ChangeScene(4);
    }

    IEnumerator EndGameWinCor() {
        // Para a corotina que conta o tempo
        StopCoroutine(collapse);
        yield return new WaitForSeconds(heartCollapseTime);
        // Acha o jogador e o destroi (junto da sua barra de vida)
        PlayerManager player = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
        this.playerScore = calculatePlayerScore(this.timeScore, player.GetHealth());
        player.DestroySelfAndHealthbar();
        // Vai para a cena dos rankings finais
        ChangeScene(5);
        Button menuButton = GameObject.FindWithTag("MenuButton").GetComponent<Button>();
        menuButton.onClick.AddListener(() => ChangeScene(0));
    }

    int calculatePlayerScore(float timeScore, float health) {
        int score = Mathf.RoundToInt(timeScore*60);
        score += Mathf.RoundToInt(health*240);
        return score;
    }

    public int GetPlayerScore() {
        return this.playerScore;
    }
}
