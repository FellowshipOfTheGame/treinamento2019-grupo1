﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossAttack : MonoBehaviour {

    // Em todos os arrays float e int abaixo, a posicao [0] se refere
    // ao ataque "light" do boss e a posicao [1] ao ataque "heavy".
    public bool canAttack = true;
    private FirstBossManager manager;
    public Transform[] lightAttack;
    public Transform[] heavyAttack;
    public SpriteRenderer attackProjSprite;
    public LayerMask playerLayer;
    public LayerMask columnLayer;
    private Transform player;
    private Animator animator;
    public float[] attackRange;
    public int[] attackDamage;
    public float[] attackDelay;
    public Transform heavyToShow;
    private float[] curAttackDelay = {0f, 0f};
    private int attacksBeforeHeavy = -1;
    private bool hasAttacked = false;
    private BoxCollider2D normalCollider;
    private PolygonCollider2D stunnedCollider;
    private float angle;
    private Animator cameraAnimator;
    private bool alive = true;

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        manager = GetComponent<FirstBossManager>();
        animator = GetComponent<Animator>();
        normalCollider = manager.oldCollider;
        cameraAnimator = GameObject.FindWithTag("MainCamera").GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        attackProjSprite.enabled = false;
    }

    // Essa funcao e chamada a cada frame
    void Update() {
        // Se perdeu a referencia ao jogador, quer dizer que ele morreu
        if (player == null) return;
        // Se o boss pode atacar...
        if (canAttack) {
            // Se o numero de ataques para dar um forte for -1 (ou seja, o boss já deu o ataque forte), tira-se um numero aleatorio entre 2 e 5 para ser o numero de ataques leves ate o proximo ataque pesado do boss
            if (attacksBeforeHeavy == -1) {
                attacksBeforeHeavy = Random.Range(2, 6);    // o segundo parametro do Random.Range e exclusivo, entao ele esta pegando numeros inteiros pertencentes ao conjunto [2, 6[.
            }
            if (attacksBeforeHeavy > 0) {   // Ele vai tentar dar um ataque leve
                if (curAttackDelay[0] <= 0) {   // Se o boss pode dar o ataque leve de novo...
                    // Gravo o vetor que liga o boss com o jogador
                    Vector3 playerRng = player.position - normalCollider.transform.position;
                    // Decido a posicao do ataque dele
                    Vector3 attackPos = DecideAttackPos(0);
                    // E gravo o vetor que liga o boss com a posicao de ataque dele
                    Vector3 attackRng = GetAttackVector(attackPos, 0);
                    // Se o jogador estiver dentro do range de ataque do boss
                    if ( playerRng.sqrMagnitude <= attackRng.sqrMagnitude*1.2f || (playerRng-attackRng).sqrMagnitude <= (attackRange[0]*attackRange[0]) ) {
                        // Avisa o Animator que o boss atacou
                        animator.SetTrigger("LightAttack");
                        // Comeca a corotina do ataque
                        StartCoroutine(LightAttack(attackRng, attackPos));
                        // Comeca a contar o delay de ataque
                        curAttackDelay[0] = attackDelay[0];
                        hasAttacked = true;
                        manager.SetMovement(false);
                        manager.SetAttack(false);
                        // Comeca a corotina para devolver o movimento ao boss depois do tempo ter passado
                        StartCoroutine(RefreshBoss(manager, 0.7f));
                    }
                }
            }
            else {  // Ele vai tentar dar o ataque pesado, que depois ira o atordoar
                if (curAttackDelay[0] <= 0 && curAttackDelay[1] <= 0) {   // Se o boss pode dar um ataque de novo...
                    // Gravo o vetor que liga o boss com o jogador
                    Vector3 playerRng = player.position - normalCollider.transform.position;
                    // Decido a posicao do ataque dele
                    Vector3 attackPos = DecideAttackPos(1);
                    // E o vetor que liga o boss com a posicao de ataque dele
                    Vector3 attackRng = GetAttackVector(attackPos, 1);
                    // Se o jogador estiver dentro do range do ataque do boss
                    if ( playerRng.sqrMagnitude <= attackRng.sqrMagnitude || (playerRng-attackRng).sqrMagnitude <= (attackRange[1]*attackRange[1]) ) {
                        // Avisa o Animator que o boss atacou
                        animator.SetTrigger("HeavyAttack");
                        // Comeca a corotina do ataque
                        StartCoroutine(HeavyAttack(attackRng, attackPos));
                        // Comeca a contar o delay de ataque
                        curAttackDelay[1] = attackDelay[1];
                        hasAttacked = true;
                        manager.SetMovement(false);
                        manager.SetAttack(false);
                        // Comeca a corotina para devolver o movimento ao boss depois do tempo ter passado
                        StartCoroutine(RefreshBoss(manager, attackDelay[1]));
                    }
                }
            }
            // Se ele atacou, descresce-se o numero de ataques restantes para o proximo ataque pesado
            if (hasAttacked) attacksBeforeHeavy--;
            // Decresce o tempo de espera para que o boss possa dar o ataque leve de novo
            curAttackDelay[0] -= Time.deltaTime;
        }
        // Decresce o tempo de espera para que o boss possa dar o ataque pesado de novo
        curAttackDelay[1] -= Time.deltaTime;
        // Prepara para a proxima chamada
        hasAttacked = false;
    }

    Vector3 DecideAttackPos(int index) {
        // Recupero o vetor de movimento do boss
        Vector3 movement = manager.GetMovementVector();
        // Pego qual o angulo do vetor (se ele nao for o vetor nulo)
        if (movement != Vector3.zero) {
            angle = Vector3.SignedAngle(movement, Vector3.right, Vector3.back);
        }
        // Decido, a partir do parametro, de qual dos dois ataques se trata
        if (index == 0) {   // Ataque leve
            // A partir daqui, se decide para qual das direcoes o boss ira atacar
            if (angle >= 45f && angle < 135f) { // Cima
                return lightAttack[1].position;
            }
            else if (angle >= -45f && angle < 45f) {    // Direita
                return lightAttack[3].position;
            }
            else if (angle >= -135f && angle < -45f) {  // Baixo
                return lightAttack[0].position;
            }
            else {  // Esquerda
                return lightAttack[2].position;
            }
        }
        else if (index == 1) {  // Ataque pesado
            // A partir daqui, se decide para qual das direcoes o boss ira atacar
            if (angle >= 45f && angle < 135f) { // Cima
                attackProjSprite.transform.position = heavyAttack[1].position;
                return heavyAttack[1].position;
            }
            else if (angle >= -45f && angle < 45f) {    // Direita
                attackProjSprite.transform.position = heavyAttack[3].position;
                return heavyAttack[3].position;
            }
            else if (angle >= -135f && angle < -45f) {  // Baixo
                attackProjSprite.transform.position = heavyAttack[0].position;
                return heavyAttack[0].position;
            }
            else {  // Esquerda
                attackProjSprite.transform.position = heavyAttack[2].position;
                return heavyAttack[2].position;
            }
        }
        else return Vector3.zero;   // Erro
    }

    // Retorna um vetor que se extende ate o final do circulo de impacto do ataque em questao
    Vector3 GetAttackVector(Vector3 centerPos, int index) {
        Vector3 attack = centerPos - this.transform.position;
        attack += attack.normalized*attackRange[index];
        attack.z = 0f;
        return attack;
    }

    // Corotina utilizada para controlar o ataque leve do boss
    IEnumerator LightAttack(Vector3 attack, Vector3 position) {
        // Toca o som do ar gerado pelo ataque
        AudioManager.instance.Play("FirstBossAttackSlash");
        // Pausa e volta depois de 0,3 segundos
        yield return new WaitForSeconds(0.3f);
        // Cria um circulo na posicao de ataque
        Collider2D[] playersToDamage = Physics2D.OverlapCircleAll(position, attackRange[0], playerLayer);
        Collider2D column = Physics2D.OverlapCircle(position, attackRange[0], columnLayer);
        // Todos os Colliders (jogadores) encontrados sofrem dano
        foreach (Collider2D playerCol in playersToDamage) {
            //playerCol.SendMessage("TryToDefend", position);
            playerCol.SendMessage("TakeDamage", attackDamage[0]);
            // Toca o som de acerto do ataque
            AudioManager.instance.Play("FirstBossAttack");
            // Empurra o jogador para longe
            Rigidbody2D playerRB = playerCol.attachedRigidbody;
            playerRB.MovePosition(playerRB.position + (Vector2)attack*1.5f);
            // O jogador fica impossibilitado de se mover por um tempo
            PlayerManager pManager = playerCol.GetComponent<PlayerManager>();
            pManager.SetMovement(false);
            pManager.SetAttack(false);
            // Comeca a corotina para devolver o movimento ao jogador depois do tempo ter passado
            StartCoroutine(RefreshPlayer(pManager, 0));
        }
        // Se o collider do pilar foi encontrado, ele sofre dano
        if (column != null) {
            column.SendMessage("TakeDamage", attackDamage[0]);
            AudioManager.instance.Play("FirstBossAttack");
        }
    }

    // Corotina utilizada para controlar o ataque pesado do boss
    IEnumerator HeavyAttack(Vector3 attack, Vector3 position) {
        // Cria um circulo na posicao de ataque para mostrar a area que o ataque afetara
        attackProjSprite.enabled = true;
        // Pausa e volta depois de um tempo
        yield return new WaitForSeconds(0.5f);
        attackProjSprite.enabled = false;
        // Cria um circulo na posicao de ataque para pegar todos os colliders que caem dentro dele
        Collider2D[] playersToDamage = Physics2D.OverlapCircleAll(position, attackRange[1], playerLayer);
        Collider2D column = Physics2D.OverlapCircle(position, attackRange[1], columnLayer);
        // Todos os Colliders (jogadores) encontrados sofrem dano
        foreach (Collider2D playerCol in playersToDamage) {
            //playerCol.SendMessage("TryToDefend", position);
            playerCol.SendMessage("TakeDamage", attackDamage[1]);
            // O jogador fica impossibilitado de se mover por um tempo
            PlayerManager pManager = playerCol.GetComponent<PlayerManager>();
            pManager.SetMovement(false);
            pManager.SetAttack(false);
            // Comeca a corotina para devolver o movimento ao jogador depois do tempo ter passado
            StartCoroutine(RefreshPlayer(pManager, 1));
        }
        // Se o collider do pilar foi encontrado, ele sofre dano
        if (column != null) {
            column.SendMessage("TakeDamage", attackDamage[1]);
        }
        // Toca o som do ataque
        AudioManager.instance.Play("FirstBossAttack");
        // Avisa a Camera para tremer
        cameraAnimator.SetTrigger("Shake");
        // Espera por 0,5 segundos, até que o boss esteja no seu ultimo frame de ataque, para trocar seu Collider
        yield return new WaitForSeconds(0.5f);
        normalCollider.enabled = false;
        stunnedCollider = this.gameObject.AddComponent<PolygonCollider2D>();
    }

    // Retoma o comando do jogador ao mesmo
    IEnumerator RefreshPlayer(PlayerManager pManager, int index) {
        float timer = attackDelay[index]/2;
        bool turnPlayerColor = false;
        while (timer > 0) {
            timer -= 0.1f;
            turnPlayerColor = !turnPlayerColor;
            if (turnPlayerColor) pManager.SetColor(Color.clear);
            else pManager.SetColor(Color.white);
            yield return new WaitForSeconds(0.1f);
        }
        pManager.SetMovement(true);
        pManager.SetAttack(true);
        pManager.SetColor(Color.white);
    }

    // Retoma o comando do boss ao mesmo
    IEnumerator RefreshBoss(FirstBossManager manager, float timeToStop) {
        float timer = timeToStop;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (alive) {
            manager.SetMovement(true);
            manager.SetAttack(true);
            normalCollider.enabled = true;
            Destroy(stunnedCollider);
        } 
    }

    // Essa funcao permite visualizar na "Scene View" as bolinhas de colisao
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        for (int i = 0; i < 4; i++) {
            Gizmos.DrawWireSphere(lightAttack[i].position, attackRange[0]);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(heavyToShow.position, attackRange[1]);
    }

    public void IsAlive(bool answer) {
        this.alive = answer;
    }
}
