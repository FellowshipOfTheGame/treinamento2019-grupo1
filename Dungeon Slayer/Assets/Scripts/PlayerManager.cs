using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    public PlayerMovement movementScript;
    public PlayerAttack attackScript;
    public Animator animator;
    [SerializeField] private float health;

    // Esta funcao e chamada a cada frame
    void Update() {
        if (health <= 0) {
            // O jogador morreu
            Destroy(gameObject);    // Destroi o jogador
            // Chama a cena de Fim de Jogo
        }
    }
    
    // Funcao que sera chamada sempre que o jogador tiver de receber dano
    public void TakeDamage(int amount) {
        if (health > 0) health -= amount;
        // animator.SetTrigger("HasTakenDamage");
        Debug.Log("Damage Taken!");
    }

    public void SetMovement(bool canPlayerMove) {
        movementScript.canMove = canPlayerMove;
    }

    public void SetAttack(bool canPlayerAttack) {
        attackScript.canAttack = canPlayerAttack;
    }
}
