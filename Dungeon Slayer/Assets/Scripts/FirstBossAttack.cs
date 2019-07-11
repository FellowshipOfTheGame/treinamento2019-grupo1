using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossAttack : MonoBehaviour {

    // Em todos os arrays float e int abaixo, a posicao [0] se refere
    // ao ataque "light" do boss e a posicao [1] ao ataque "heavy".
    public bool canAttack = true;
    public FirstBossManager manager;
    public Transform[] lightAttack;
    public Transform[] heavyAttack;
    public LayerMask playerLayer;
    public Transform player;
    public Animator animator;
    public float[] attackRange;
    public int[] attackDamage;
    public float[] attackDelay;
    private float[] curAttackDelay = {0f, 0f};
    private int attacksBeforeHeavy = -1;
    public Transform showcase1;
    public Transform showcase2;
    private bool hasAttacked = false;

    // Essa funcao e chamada a cada frame
    void Update() {
        // Se o boss pode atacar e o jogador ainda esta vivo...
        if (canAttack && player != null) {
            // Se o numero de ataques para dar um forte for -1 (ou seja, o boss já deu o ataque forte), tira-se um numero aleatorio entre 1 e 5 para ser o numero de ataques leves ate o proximo ataque pesado do boss
            if (attacksBeforeHeavy == -1) {
                attacksBeforeHeavy = Random.Range(1, 6);    // o segundo parametro do Random.Range e exclusivo, entao ele esta pegando numeros inteiros pertencentes ao conjunto [1, 6[.
            }
            if (attacksBeforeHeavy > 0) {   // Ele vai tentar dar um ataque leve
                if (curAttackDelay[0] <= 0) {   // Se o boss pode dar o ataque leve de novo...
                    // Gravo o vetor que liga o boss com o jogador
                    Vector3 attack = player.position - this.transform.position;
                    // Se o jogador estiver dentro do range do ataque do boss
                    if (attack.sqrMagnitude <= 2*attackRange[0]*attackRange[0]) {
                        // Avisa o Animator que o boss atacou
                        animator.SetTrigger("LightAttack");
                        // Pego qual o angulo do vetor
                        float angle = Vector3.SignedAngle(attack, Vector3.right, Vector3.forward);
                        // Instancio um vetor de colisoes
                        Collider2D[] playersToDamage;
                        // A partir daqui, se decide para qual das direcoes o boss ira atacar
                        if (angle > 225f && angle <= 315) {  // Baixo
                            // Cria um circulo na posicao de ataque
                            playersToDamage = Physics2D.OverlapCircleAll(lightAttack[0].position, attackRange[0], playerLayer);
                        }
                        else if (angle > 45f && angle <= 135f) {    // Cima
                            // Cria um circulo na posicao de ataque
                            playersToDamage = Physics2D.OverlapCircleAll(lightAttack[1].position, attackRange[0], playerLayer);
                        }
                        else if (angle > 135f && angle <= 225f) {   // Esquerda
                            // Cria um circulo na posicao de ataque
                            playersToDamage = Physics2D.OverlapCircleAll(lightAttack[2].position, attackRange[0], playerLayer);
                        }
                        else {  // Direita
                            // Cria um circulo na posicao de ataque
                            playersToDamage = Physics2D.OverlapCircleAll(lightAttack[3].position, attackRange[0], playerLayer);
                        }
                        // Todos os Colliders (jogadores) encontrados sofrem dano
                        foreach (Collider2D playerCol in playersToDamage) {
                            playerCol.SendMessage("TakeDamage", attackDamage[0]);
                            // Empurra o jogador para longe
                            playerCol.GetComponent<Rigidbody2D>().AddForce(attack, ForceMode2D.Impulse);   
                        }
                        // Comeca a contar o delay de ataque
                        curAttackDelay[0] = attackDelay[0];
                        hasAttacked = true;
                    }
                }
            }
            else {  // Ele vai tentar dar o ataque pesado, que depois ira o atordoar
                if (curAttackDelay[1] <= 0) {   // Se o boss pode dar o ataque pesado de novo...
                    // Gravo o vetor que liga o boss com o jogador
                    Vector3 attack = player.position - this.transform.position;
                    // Se o jogador estiver dentro do range do ataque do boss
                    if (attack.sqrMagnitude <= 2*attackRange[1]*attackRange[1]) {
                        // Avisa o Animator que o boss atacou
                        animator.SetTrigger("HeavyAttack");
                        // Pego qual o angulo do vetor
                        float angle = Vector3.SignedAngle(attack, Vector3.right, Vector3.forward);
                        // Instancio um vetor de colisoes
                        Collider2D[] playersToDamage;
                        // A partir daqui, se decide para qual das direcoes o boss ira atacar
                        if (angle > 225f && angle <= 315) {  // Baixo
                            // Cria um circulo na posicao de ataque
                            playersToDamage = Physics2D.OverlapCircleAll(heavyAttack[0].position, attackRange[1], playerLayer);
                        }
                        else if (angle > 45f && angle <= 135f) {    // Cima
                            // Cria um circulo na posicao de ataque
                            playersToDamage = Physics2D.OverlapCircleAll(heavyAttack[1].position, attackRange[1], playerLayer);
                        }
                        else if (angle > 135f && angle <= 225f) {   // Esquerda
                            // Cria um circulo na posicao de ataque
                            playersToDamage = Physics2D.OverlapCircleAll(heavyAttack[2].position, attackRange[1], playerLayer);
                        }
                        else {  // Direita
                            // Cria um circulo na posicao de ataque
                            playersToDamage = Physics2D.OverlapCircleAll(heavyAttack[3].position, attackRange[1], playerLayer);
                        }
                        // Todos os Colliders (jogadores) encontrados sofrem dano
                        foreach (Collider2D playerCol in playersToDamage) {
                            playerCol.SendMessage("TakeDamage", attackDamage[1]);
                            // O jogador fica impossibilitado de se mover por um tempo
                            PlayerManager pManager = player.GetComponent<PlayerManager>();
                            pManager.SetMovement(false);
                            pManager.SetAttack(false);
                            // Comeca a corotina para devolver o movimento ao jogador depois do tempo ter passado
                            StartCoroutine(refreshPlayer(pManager));
                        }
                        // Comeca a contar o delay de ataque
                        curAttackDelay[1] = attackDelay[1];
                        hasAttacked = true;
                        manager.SetMovement(false);
                        manager.SetAttack(false);
                        // Comeca a corotina para devolver o movimento ao boss depois do tempo ter passado
                        StartCoroutine(refreshBoss(manager));
                    }
                }
            }
            // Se ele atacou, descresce-se o numero de ataques restantes para o proximo ataque pesado
            if (hasAttacked) attacksBeforeHeavy--;
        }
        // Decresce os tempos de espera para que o boss possa atacar de novo
        curAttackDelay[0] -= Time.deltaTime;
        curAttackDelay[1] -= Time.deltaTime;
        // Prepara para a proxima chamada
        hasAttacked = false;
        Debug.Log(attacksBeforeHeavy);
    }

    IEnumerator refreshPlayer(PlayerManager manager) {
        float timer = attackDelay[1]/2;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        manager.SetMovement(true);
        manager.SetAttack(true);
    }

    IEnumerator refreshBoss(FirstBossManager manager) {
        float timer = attackDelay[1];
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        manager.SetMovement(true);
        manager.SetAttack(true);
    }

    // Essa funcao permite visualizar na "Scene View" as bolinhas de colisao
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(showcase1.position, attackRange[0]);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(showcase2.position, attackRange[1]);
    }
}
