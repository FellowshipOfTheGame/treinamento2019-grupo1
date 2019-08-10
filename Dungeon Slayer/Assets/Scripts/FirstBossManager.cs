using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossManager : MonoBehaviour {

    public FirstBossMovement movementScript;
    public FirstBossAttack attackScript;
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
        if (curHealth > 0) curHealth -= amount;
        // animator.SetTrigger("HasTakenDamage");
    }

    public void SetMovement(bool canBossMove) {
        movementScript.canMove = canBossMove;
    }

    public void SetAttack(bool canBossAttack) {
        attackScript.canAttack = canBossAttack;
    }

    public Vector3 GetMovementVector() {
        return movementScript.movement;
    }
}
