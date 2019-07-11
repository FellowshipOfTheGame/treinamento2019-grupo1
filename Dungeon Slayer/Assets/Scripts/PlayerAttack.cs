using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    
    public bool canAttack = true;
    public Transform attack;
    public LayerMask enemyLayer;
    public GameObject hitEffect;
    public Animator animator;
    public float attackRange;
    public int attackDamage;
    public float attackDelay;
    private float curAttackDelay = 0f;
    

    // Essa funcao e chamada a cada frame
    void Update() {
        if (canAttack) {
            if (curAttackDelay <= 0) {
                // O jogador pode atacar
                if (Input.GetAxisRaw("Attack") == 1) {  // Se o jogador pressionou o botao de ataque
                    // Avisa o Animator que o jogador atacou
                    animator.SetTrigger("HasAttacked");
                    // Cria um circulo na posicao de ataque
                    Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attack.position, attackRange, enemyLayer);
                    // Todos os Colliders (inimigos) encontrados sofrem dano
                    foreach (Collider2D enemy in enemiesToDamage) {
                        enemy.SendMessage("TakeDamage", attackDamage);
                        // Instancia o efeito de acerto de ataque (e o destroi depois de certo tempo)
                        Destroy(Instantiate(hitEffect, attack.position, Quaternion.identity), 0.4f);
                    }
                    // Comeca a contar o delay de ataque
                    curAttackDelay = attackDelay;
                }
            }
        }
        // Vai decrescendo o tempo de espera para que o jogador possa atacar de novo
        curAttackDelay -= Time.deltaTime;
    }

    // Essa funcao permite visualizar na "Scene View" a bolinha de colisao
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack.position, attackRange);
    }
}
