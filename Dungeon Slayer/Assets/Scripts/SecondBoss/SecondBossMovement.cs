using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossMovement : MonoBehaviour {

    public bool canMove = true;
    public SpriteRenderer sprite;
    public Rigidbody2D bossRB;
    public BoxCollider2D bossCollider;
    public ColumnManager column;
    public Animator animator;
    [HideInInspector] public Vector3 movement = Vector3.zero;
    public float speed;
    [SerializeField] private float minDistance = 0.1f;
    private float smoothTime = 0.0001f;
    private Vector3 curVelocity;
    //[SerializeField] private float detectionDistance = 4f;
    //[SerializeField] private float deviationAmount = 1f;

    // Essa funcao e chamada a cada frame
    void Update() {
        // Se o boss pode se mover...
        if (canMove) {
            // Pega a próxima posicao do pilar
            Vector3 target = column.newColumnPos();
            // O boss corre para o proximo pilar que sera levantado
            movement = GetBestMove(this.transform.position, target);
            // O vetor de movimento tem sua magnitude normalizada para que o boss sempre tenha a mesma velocidade, independente da distancia dele aos pilares ou ao jogador
            movement = movement.normalized;
            // Se o boss ja alcancou o topo do pilar...
            if (Vector3.Distance(this.transform.position, target) <= minDistance) {
                // O boss para de se mover
                movement = Vector3.zero;
            }
            // Avisa o Animator da velocidade atual do boss
            animator.SetFloat("Speed", movement.sqrMagnitude);
            // Vira a sprite do boss de acordo com o movimento que ele esta fazendo
            float dot = Vector3.Dot(movement, Vector3.right);
            if (dot < 0) sprite.flipX = false;
            else sprite.flipX = true;
        }
        else {
            // Caso ele nao possa...
            // Fica parado
            movement = Vector3.zero;
            // Avisa ao Animator que ele esta parado
            animator.SetFloat("Speed", 0);
        }
    }

    // Essa funcao e chamada a cada determinado periodo de tempo (usada para coisas que envolvem fisica)
    void FixedUpdate() {
        // Movimenta o boss para perto do player, fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento
        bossRB.velocity = Vector3.SmoothDamp(bossRB.velocity, movement*speed, ref curVelocity, smoothTime);
    }

    Vector3 GetBestMove(Vector3 cur, Vector3 dest) {
        return dest - cur;   // Vetor direto entre a posicao atual e o destino (ja que o boss nao colide com outros personagens)
    }
}
