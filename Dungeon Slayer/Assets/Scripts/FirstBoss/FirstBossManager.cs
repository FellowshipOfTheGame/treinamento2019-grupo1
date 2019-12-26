using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossManager : MonoBehaviour {

    public FirstBossMovement movementScript;
    public FirstBossAttack attackScript;
    public SecondBossManager secondBoss;
    public GameObject shield;
    public SpriteRenderer sprite;
    public Animator animator;
    public BoxCollider2D oldCollider;
    public PolygonCollider2D newCollider;
    public Slider healthBar;
    [SerializeField] private float health = 100f;
    private float curHealth;
    private GameObject[] doors;

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
        if (curHealth <= 0) {
            // O boss morreu
            movementScript.canMove = false; // O boss nao pode mais se mexer
            attackScript.canAttack = false; // O boss nao pode mais atacar
            animator.SetTrigger("Died");    // Avisa ao Animator que ele morreu
            oldCollider.enabled = false;
            newCollider.enabled = true;
            if (healthBar != null) Destroy(healthBar.gameObject, 0.5f); // Desativa sua barra de vida apos um certo tempo
            Destroy(gameObject, 4f);    // Destroi ele apos um certo tempo
            // Abre todas as portas para o jogador
            foreach (GameObject obj in doors) obj.SetActive(true);
        }
        // Atualiza a barra de vida
        if (healthBar != null) healthBar.SetValueWithoutNotify(curHealth/health);
    }

    // Funcao que sera chamada sempre que o boss tiver de receber dano
    public void TakeDamage(int amount) {
        // Se o segundo boss nao esta vivo (ou seja, nao tem escudo protegendo este boss)
        if (secondBoss == null) {
            if (curHealth > 0) curHealth -= amount; // Se é possível tirar vida dele, tire
            StartCoroutine(SwitchColor(0.1f));
        }
    }

    // Funcao para que o boss tente se defender
    public void TryToDefend(Vector3 position) {
        // Se o segundo boss esta vivo
        if (secondBoss != null) {
            // O escudo protege este boss
            Destroy(Instantiate(shield, position, Quaternion.identity), 0.4f);
            EventsManager.current.FirstBossHit();  // Ativa os eventos em resposta ao boss tomar um hit
        }
    }

    public void SetMovement(bool canBossMove) {
        movementScript.canMove = canBossMove;
    }

    public void SetAttack(bool canBossAttack) {
        attackScript.canAttack = canBossAttack;
    }

    public void SetColor(Color c) {
        if (sprite != null) sprite.color = c;
    }

    public Vector3 GetMovementVector() {
        return movementScript.movement;
    }

    // Faz com que o boss fique trocando de cor para sinalizar que ele levou o hit
    IEnumerator SwitchColor(float timer) {
        bool turnBossColor = false;
        while (timer > 0) {
            timer -= 0.1f;
            turnBossColor = !turnBossColor;
            if (turnBossColor) SetColor(Color.red);
            else SetColor(Color.white);
            yield return new WaitForSeconds(0.1f);
        }
        SetColor(Color.white);
    }
}
