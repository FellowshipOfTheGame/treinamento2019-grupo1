using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossManager : MonoBehaviour {

    public FirstBossMovement movementScript;
    public FirstBossAttack attackScript;
    public Animator animator;
    public BoxCollider2D oldCollider;
    public PolygonCollider2D newCollider;
    [SerializeField] private float health;

    // Esta funcao e chamada a cada frame
    void Update() {
        if (health <= 0) {
            // O boss morreu
            movementScript.canMove = false; // O boss nao pode mais se mexer
            animator.SetTrigger("Died");    // Avisa ao Animator que ele morreu
            oldCollider.enabled = false;
            newCollider.enabled = true;
            Destroy(gameObject, 4f);    // Destroi ele apos um certo tempo
        }
    }

    // Funcao que sera chamada sempre que o boss tiver de receber dano
    public void TakeDamage(int amount) {
        if (health > 0) health -= amount;
        // animator.SetTrigger("HasTakenDamage");
    }

    public void SetMovement(bool canBossMove) {
        movementScript.canMove = canBossMove;
    }

    public void SetAttack(bool canBossAttack) {
        attackScript.canAttack = canBossAttack;
    }
}
