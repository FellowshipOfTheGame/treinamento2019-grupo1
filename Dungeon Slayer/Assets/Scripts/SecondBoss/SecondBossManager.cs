using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondBossManager : MonoBehaviour {

    public SecondBossMovement movementScript;
    //public SecondBossAttack attackScript;
    //public Animator animator;
    public BoxCollider2D col;
    public Slider healthBar;
    [SerializeField] private float health = 20f;
    private float curHealth;
    private bool isOnGround = true;

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health; // Inicializa a vida do boss
        // Tirar isso depois
        StartCoroutine(SwitchGround());
    }

    IEnumerator SwitchGround() {
        while (true) {
            yield return new WaitForSeconds(5f);
            isOnGround = !isOnGround;
        }
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (curHealth <= 0) {
            // O boss morreu
            movementScript.canMove = false; // O boss nao pode mais se mexer
            //attackScript.canAttack = false; // O boss nao pode mais atacar
            //animator.SetTrigger("Died");    // Avisa ao Animator que ele morreu
            if (healthBar != null) Destroy(healthBar.gameObject, 0.5f); // Desativa sua barra de vida apos um certo tempo
            Destroy(gameObject, 0.5f);    // Destroi ele apos um certo tempo
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

    /*
    public void SetAttack(bool canBossAttack) {
        attackScript.canAttack = canBossAttack;
    }

    public Vector3 GetMovementVector() {
        return movementScript.movement;
    }
    */
}
