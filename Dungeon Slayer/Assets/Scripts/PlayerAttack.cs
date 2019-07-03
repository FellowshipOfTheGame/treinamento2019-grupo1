using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attack;
    public LayerMask enemyLayer;
    public GameObject hitEffect;
    public float attackRange;
    public int attackDamage;
    private float attackDelay;
    public float initAttackDelay;

    // Essa funcao e chamada a cada frame
    void Update()
    {
        if (attackDelay <= 0) {
            // O jogador pode atacar
            if (Input.GetAxisRaw("Attack") == 1) {  // Se o jogador pressionou o botao de ataque
                // Cria um circulo na posicao de ataque
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attack.position, attackRange, enemyLayer);
                // Todos os Colliders (inimigos) encontrados sofrem dano
                foreach (Collider2D enemy in enemiesToDamage) {
                    Debug.Log(enemy);
                    // TODO: o inimigo sofre dano
                }

                // Instancia o efeito de acerto de ataque (e o destroi depois de certo tempo)
                Destroy(Instantiate(hitEffect, attack.position, Quaternion.identity), 0.4f);   // TODO: colocar dentro do foreach
                attackDelay = initAttackDelay;
            }
        }
        else {  // Vai decrescendo o tempo de espera para que o jogador possa atacar de novo
            attackDelay -= Time.deltaTime;
        }
    }

    // Essa funcao permite visualizar na "Scene View" a bolinha de colisao
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack.position, attackRange);
    }
}
