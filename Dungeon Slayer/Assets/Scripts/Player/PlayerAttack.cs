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
                    // Toca o som de ataque do jogador
                    AudioManager.instance.Play("PlayerAttack");
                    // Comeca a corotina para instanciar o ataque
                    StartCoroutine(SpawnHit());
                    // Comeca a contar o delay de ataque
                    curAttackDelay = attackDelay;
                }
            }
        }
        // Vai decrescendo o tempo de espera para que o jogador possa atacar de novo
        curAttackDelay -= Time.deltaTime;
    }

    IEnumerator SpawnHit() {
        // Espera 0.3 segundos
        yield return new WaitForSeconds(0.3f);
        // Cria um circulo na posicao de ataque
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attack.position, attackRange);
        // Todos os Colliders encontrados sofrem o hit
        foreach (Collider2D enemy in enemiesToDamage) {
            // Instancia o efeito de acerto de ataque (e o destroi depois de certo tempo)
            Destroy(Instantiate(hitEffect, attack.position, Quaternion.identity), 0.4f);
            // Toca o som de acerto do ataque
            AudioManager.instance.Play("SwordSlash");
            // Se o ataque acertou em um inimigo
            if (enemy.tag == "Enemy" || enemy.tag == "Boss") {
                enemy.SendMessage("TryToDefend", attack.position, SendMessageOptions.DontRequireReceiver);  // Primeiro, ele tenta se defender
                enemy.SendMessage("TakeDamage", attackDamage);  // Caso nao consiga, ele toma dano
            }
        }
    }

    // Essa funcao permite visualizar na "Scene View" a bolinha de colisao
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack.position, attackRange);
    }
}
