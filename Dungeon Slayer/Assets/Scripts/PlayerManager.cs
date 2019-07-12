using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {
    
    public PlayerMovement movementScript;
    public PlayerAttack attackScript;
    public Animator animator;
    public SpriteRenderer sprite;
    public Slider healthBar;
    [SerializeField] private float health = 30f;
    private float curHealth;

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health;
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (curHealth <= 0) {
            // O jogador morreu
            Destroy(gameObject);    // Destroi o jogador
            // Chama a cena de Fim de Jogo
            Application.Quit();
        }
        // Atualiza a barra de vida
        healthBar.SetValueWithoutNotify(curHealth/health);
    }
    
    // Funcao que sera chamada sempre que o jogador tiver de receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        // animator.SetTrigger("HasTakenDamage");
    }

    public void SetMovement(bool canPlayerMove) {
        movementScript.canMove = canPlayerMove;
    }

    public void SetAttack(bool canPlayerAttack) {
        attackScript.canAttack = canPlayerAttack;
    }

    public void SetColor(Color c) {
        if (sprite != null) sprite.color = c;
    }
}
