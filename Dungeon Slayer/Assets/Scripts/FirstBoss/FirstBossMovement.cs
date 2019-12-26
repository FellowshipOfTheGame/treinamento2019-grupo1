using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossMovement : MonoBehaviour {

    public bool canMove = true;
    public Rigidbody2D bossRB;
    public BoxCollider2D bossCollider;
    private Transform player;
    public Animator animator;
    [HideInInspector] public Vector3 movement = Vector3.zero;
    public float speed;
    [SerializeField] private float minDistance = 10f;
    private float smoothTime = 0.0001f;
    private Vector3 curVelocity;
    private bool isWalkSoundPlaying;

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        player = GameObject.FindWithTag("Player").transform;
        // Comeca a tocar o som do andar do boss
        AudioManager.instance.Play("FirstBossWalk");
        isWalkSoundPlaying = true;
    }

    // Essa funcao e chamada a cada frame
    void Update() {
        // Se perdeu a referencia ao jogador, quer dizer que ele morreu
        if (player == null) return;
        // Se o boss pode se mover...
        if (canMove) {
            // Avisa o Animator da direcao dele (antes de atualiza-la), porem apenas se ele estiver se movendo
            if (movement != Vector3.zero) {
                animator.SetFloat("LastHorizontal", movement[0]);
                animator.SetFloat("LastVertical", movement[1]);
            }
            // Um novo vetor de movimento e construido de modo a aproximar o boss do jogador
            movement = GetBestMove(this.transform.position, player.position);
            // O vetor de movimento tem sua magnitude normalizada para que o boss sempre tenha a mesma velocidade, independente da distancia dele ao jogador
            movement = movement.normalized;
            // Se o boss ja alcancou o jogador...
            if (Vector3.Distance(this.transform.position, player.position) <= minDistance) {
                // O boss continua encarando-o, porem agora esta praticamente parado
                movement /= 1000000;
            }
            // Avisa o Animator da direcao e velocidade atuais do boss
            animator.SetFloat("Horizontal", movement[0]);
            animator.SetFloat("Vertical", movement[1]);
            animator.SetFloat("Speed", movement.sqrMagnitude);
            // Verifica se o som de caminhar esta tocando
            if (!isWalkSoundPlaying) {
                // Comeca a tocar o som do andar do boss
                AudioManager.instance.Play("FirstBossWalk");
                isWalkSoundPlaying = true;
            }
        }
        else {
            // Caso ele nao possa...
            // Fica parado
            movement = Vector3.zero;
            // Avisa ao Animator que ele esta parado
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            animator.SetFloat("Speed", 0);
            // Verifica se o som de caminhar nao esta tocando
            if (isWalkSoundPlaying) {
                // Paro de tocar o som do andar do boss
                AudioManager.instance.Stop("FirstBossWalk");
                isWalkSoundPlaying = false;
            }
        }
    }

    // Essa funcao e chamada a cada determinado periodo de tempo (usada para coisas que envolvem fisica)
    void FixedUpdate() {
        // Movimenta o boss para perto do player, fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento
        bossRB.velocity = Vector3.SmoothDamp(bossRB.velocity, movement*speed, ref curVelocity, smoothTime);
    }

    Vector3 GetBestMove(Vector3 cur, Vector3 dest) {
        Vector3 ideal = dest - cur;   // Calcula o vetor ideal de movimento 
        
        // Daqui para baixo eh tudo bem questionavel
        float radius = (ideal).magnitude;    // Decide qual sera o raio do circulo que tem o boss como centro e o jogador em cima da circunferência
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(cur, radius-0.1f); // Pega todos os Colliders (obstaculos) que estao dentro deste circulo (coloquei o -0.1 para evitar de contar o jogador em si)
        
        if (obstacles.Length == 1) {    // Se apenas tem um obstaculo no caminho (caso desejado)
            return ideal;   // (solucao provisorio para nao dar compile error)
        }
        else return ideal;   // Nao vale a pena fazer os calculos. O boss pode seguir em linha reta.
    }
}
