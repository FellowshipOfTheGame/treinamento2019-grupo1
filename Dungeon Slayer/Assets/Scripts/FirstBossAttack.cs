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
    public Transform[] showcase;
    private bool hasAttacked = false;
    private BoxCollider2D normalCollider;
    private PolygonCollider2D stunnedCollider;

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        normalCollider = manager.oldCollider;
    }

    // Essa funcao e chamada a cada frame
    void Update() {
        // Se o boss pode atacar...
        if (canAttack) {
            // Se o numero de ataques para dar um forte for -1 (ou seja, o boss já deu o ataque forte), tira-se um numero aleatorio entre 2 e 5 para ser o numero de ataques leves ate o proximo ataque pesado do boss
            if (attacksBeforeHeavy == -1) {
                attacksBeforeHeavy = Random.Range(2, 6);    // o segundo parametro do Random.Range e exclusivo, entao ele esta pegando numeros inteiros pertencentes ao conjunto [2, 6[.
            }
            if (attacksBeforeHeavy > 0) {   // Ele vai tentar dar um ataque leve
                if (curAttackDelay[0] <= 0) {   // Se o boss pode dar o ataque leve de novo...
                    // Gravo o vetor que liga o boss com o jogador
                    Vector3 playerRng = player.position - this.transform.position;
                    // Dedido a posicao do ataque dele
                    Vector3 attackPos = DecideAttackPos(0);
                    // E gravo o vetor que liga o boss com a posicao de ataque dele
                    Vector3 attackRng = GetAttackVector(attackPos, 0);
                    // Se o jogador estiver dentro do range de ataque do boss
                    Debug.Log((playerRng.sqrMagnitude <= attackRng.sqrMagnitude) + " && " + ((playerRng-attackRng).sqrMagnitude <= (4*attackRange[0]*attackRange[0])));
                    if (playerRng.sqrMagnitude <= attackRng.sqrMagnitude) {
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
                    Vector3 playerRng = player.position - this.transform.position;
                    // Dedido a posicao do ataque dele
                    Vector3 attackPos = DecideAttackPos(1);
                    // E o vetor que liga o boss com a posicao de ataque dele
                    Vector3 attackRng = GetAttackVector(attackPos, 1);
                    // Se o jogador estiver dentro do range do ataque do boss
                    Debug.Log((playerRng.sqrMagnitude <= attackRng.sqrMagnitude) + " && " + ((playerRng-attackRng).sqrMagnitude <= (4*attackRange[1]*attackRange[1])));
                    if (playerRng.sqrMagnitude <= attackRng.sqrMagnitude) {
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
        // Pego qual o angulo do vetor
        float angle = Vector3.SignedAngle(movement, Vector3.right, Vector3.forward);
        // Decido, a partir do parametro, de qual dos dois ataques se trata
        if (index == 0) {   // Ataque leve
            // A partir daqui, se decide para qual das direcoes o boss ira atacar
            if (angle > 225f && angle <= 315) {  // Baixo
                return lightAttack[0].position;
            }
            else if (angle > 45f && angle <= 135f) {    // Cima
                return lightAttack[1].position;
            }
            else if (angle > 135f && angle <= 225f) {   // Esquerda
                return lightAttack[2].position;
            }
            else {  // Direita
                return lightAttack[3].position;
            }
        }
        else if (index == 1) {  // Ataque pesado
            // A partir daqui, se decide para qual das direcoes o boss ira atacar
            if (angle > 225f && angle <= 315) {  // Baixo
                return heavyAttack[0].position;
            }
            else if (angle > 45f && angle <= 135f) {    // Cima
                return heavyAttack[1].position;
            }
            else if (angle > 135f && angle <= 225f) {   // Esquerda
                return heavyAttack[2].position;
            }
            else {  // Direita
                return heavyAttack[3].position;
            }
        }
        else return Vector3.zero;   // Erro
    }

    // Retorna um vetor que se extende ate o final do circulo de impacto do ataque em questao
    Vector3 GetAttackVector(Vector3 centerPos, int index) {
        Vector3 attack = centerPos - this.transform.position;
        attack += attack.normalized*attackRange[index];
        return attack;
    }

    // Corotina utilizada para controlar o ataque leve do boss
    IEnumerator LightAttack(Vector3 attack, Vector3 position) {
        // Toca o som do ar gerado pelo ataque
        AudioManager.instance.Play("FirstBossAttackSlash");
        // Pausa e volta depois de 0,5 segundos
        yield return new WaitForSeconds(0.5f);
        // Cria um circulo na posicao de ataque
        Collider2D[] playersToDamage = Physics2D.OverlapCircleAll(position, attackRange[0], playerLayer);
        // Todos os Colliders (jogadores) encontrados sofrem dano
        foreach (Collider2D playerCol in playersToDamage) {
            playerCol.SendMessage("TakeDamage", attackDamage[0]);
            // Toca o som de acerto do ataque
            AudioManager.instance.Play("FirstBossAttack");
            // Empurra o jogador para longe
            playerCol.GetComponent<Rigidbody2D>().AddForce(attack*10, ForceMode2D.Impulse);
        }
    }

    // Corotina utilizada para controlar o ataque pesado do boss
    IEnumerator HeavyAttack(Vector3 attack, Vector3 position) {
        // Pausa e volta depois de 0,25 segundos
        yield return new WaitForSeconds(0.25f);
        // Cria um circulo na posicao de ataque
        Collider2D[] playersToDamage = Physics2D.OverlapCircleAll(position, attackRange[1], playerLayer);
        // Todos os Colliders (jogadores) encontrados sofrem dano
        foreach (Collider2D playerCol in playersToDamage) {
            playerCol.SendMessage("TakeDamage", attackDamage[1]);
            // O jogador fica impossibilitado de se mover por um tempo
            PlayerManager pManager = player.GetComponent<PlayerManager>();
            pManager.SetMovement(false);
            pManager.SetAttack(false);
            // Comeca a corotina para devolver o movimento ao jogador depois do tempo ter passado
            StartCoroutine(RefreshPlayer(pManager));
        }
        // Toca o som do ataque
        AudioManager.instance.Play("FirstBossAttack");
        yield return new WaitForSeconds(0.75f);
        normalCollider.enabled = false;
        stunnedCollider = this.gameObject.AddComponent<PolygonCollider2D>();
    }

    // Retoma o comando do jogador ao mesmo
    IEnumerator RefreshPlayer(PlayerManager pManager) {
        float timer = attackDelay[1]/2;
        bool turnPlayerColor = false;
        while (timer > 0) {
            timer -= Time.deltaTime;
            turnPlayerColor = !turnPlayerColor;
            if (turnPlayerColor) pManager.SetColor(Color.clear);
            else pManager.SetColor(Color.white);
            yield return null;
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
        manager.SetMovement(true);
        manager.SetAttack(true);
        normalCollider.enabled = true;
        Destroy(stunnedCollider);
    }

    // Essa funcao permite visualizar na "Scene View" as bolinhas de colisao
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(showcase[0].position, attackRange[0]);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(showcase[1].position, attackRange[1]);
    }
}
