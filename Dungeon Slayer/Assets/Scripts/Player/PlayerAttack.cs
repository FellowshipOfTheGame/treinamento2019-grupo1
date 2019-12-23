using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    
    public bool canAttack = true;
    private PlayerManager manager;
    public Transform[] attack;
    public LayerMask enemyLayer;
    public GameObject hitEffect;
    public Animator animator;
    public float attackRange;
    public int attackDamage;
    public float attackDelay;
    private float curAttackDelay = 0f;
    private float angle = 0f;
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        manager = GetComponent<PlayerManager>();
    }

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

    Vector3 DecideAttackPos() {
        // Recupero o vetor de movimento do jogador
        Vector3 movement = manager.GetMovement();
        // Pego qual o angulo do vetor (se ele nao for o vetor nulo)
        if (movement != Vector3.zero) {
            angle = Vector3.SignedAngle(movement, Vector3.right, Vector3.back);
        }
        else {
            Vector3 lastMov = manager.GetLastMovement();
            angle = Vector3.SignedAngle(lastMov, Vector3.right, Vector3.back);
        }
        Debug.Log(angle);
        // A partir daqui, se decide para qual das direcoes o jogador ira atacar
        if (angle >= 0f && angle <= 90f) { // Diag. Sup. Dir.
            return attack[0].position;
        }
        else if (angle > 90f && angle <= 180f) {    // Diag. Sup. Esq.
            return attack[1].position;
        }
        else if (angle > -90f && angle < 0f) {  // Diag. Inf. Dir.
            return attack[3].position;
        }
        else {  // Diag. Inf. Esq.
            return attack[2].position;
        }
    }

    IEnumerator SpawnHit() {
        // Espera 0.3 segundos
        yield return new WaitForSeconds(0.3f);
        // Decide a posicao do ataque dele
        Vector3 attackPos = DecideAttackPos();
        // Cria um circulo na posicao de ataque
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos, attackRange);
        // Todos os Colliders encontrados sofrem o hit
        foreach (Collider2D enemy in enemiesToDamage) {
            if (enemy.tag == "Player") continue;
            // Instancia o efeito de acerto de ataque (e o destroi depois de certo tempo)
            Destroy(Instantiate(hitEffect, attackPos, Quaternion.identity), 0.4f);
            // Toca o som de acerto do ataque
            AudioManager.instance.Play("SwordSlash");
            // Se o ataque acertou em um inimigo
            if (enemy.tag == "Enemy" || enemy.tag == "Boss") {
                enemy.SendMessage("TryToDefend", attackPos, SendMessageOptions.DontRequireReceiver);  // Primeiro, ele tenta se defender
                enemy.SendMessage("TakeDamage", attackDamage);  // Caso nao consiga, ele toma dano
            }
            else if (enemy.tag == "Column") {
                enemy.SendMessage("TakeDamage", 1);  // Da um dano minimo no pilar
            }
        }
    }

    // Essa funcao permite visualizar na "Scene View" a bolinha de colisao
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        for (int i = 0; i < 4; i++) {
            Gizmos.DrawWireSphere(attack[i].position, attackRange);
        }
    }
}
