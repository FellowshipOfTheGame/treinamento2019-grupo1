using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonScript : MonoBehaviour {
    
    public Slider healthBar;
    public Text timeText;
    public Animator cameraAnimator;
    [SerializeField] private float health = 10f;
    [SerializeField] private float timeToEscape = 20f;
    private float curHealth;
    private GameObject[] doors;
    private bool isAlive = true;
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health;
        // Acha todas as portas da cena
        doors = GameObject.FindGameObjectsWithTag("Door");
        // Desativa todas
        foreach (GameObject obj in doors) obj.SetActive(false);
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (curHealth <= 0 && isAlive) {
            // A dungeon morreu
            if (healthBar != null) Destroy(healthBar.gameObject, 0.2f); // Desativa sua barra de vida apos um certo tempo
            timeText.gameObject.SetActive(true);    // Ativa o texto que ira mostrar o tempo restante para sair da Dungeon
            DontDestroyOnLoad(timeText.transform.parent.gameObject); // Faz com que o texto nao seja destruido ao se trocar de cena
            DontDestroyOnLoad(gameObject);    // Faz com que este objeto nao seja destruido ao se trocar de cena (para preservar o script8)
            Destroy(GetComponent<SpriteRenderer>());    // Destroi a sprite do coracao
            Destroy(GetComponent<CircleCollider2D>());    // Destroi o collider do coracao (ele ira carregar, literalmente, so o script)
            StartCoroutine(Collapse(timeToEscape));
            // Abre todas as portas para o jogador
            foreach (GameObject obj in doors) obj.SetActive(true);
            isAlive = false;
        }
        // Atualiza a barra de vida
        if (healthBar != null) healthBar.SetValueWithoutNotify(curHealth/health);
    }

    // Funcao que sera chamada sempre que o boss tiver de receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        cameraAnimator.SetTrigger("Shake"); // Avisa a Camera para tremer
        // animator.SetTrigger("HasTakenDamage");
    }

    private IEnumerator Collapse(float initTime) {
        float curTime = initTime;
        string minutes, seconds;
        while (curTime > 0) {
            minutes = Mathf.Floor(curTime/60).ToString("00");
            seconds = Mathf.Floor(curTime%60).ToString("00");
            timeText.text = minutes + ":" + seconds;
            curTime -= Time.deltaTime;
            yield return null;
        }
        Application.Quit();
    }
}
