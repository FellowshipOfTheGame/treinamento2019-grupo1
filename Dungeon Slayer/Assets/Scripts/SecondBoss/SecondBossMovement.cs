using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossMovement : MonoBehaviour {

    public bool canMove = true;
    public Rigidbody2D bossRB;
    public BoxCollider2D bossCollider;
    public ColumnManager column;
    //public Animator animator;
    [HideInInspector] public Vector3 movement = Vector3.zero;
    public float speed;
    private float smoothTime = 0.0001f;
    private Vector3 curVelocity;

    // Essa funcao e chamada a cada frame
    void Update() {
        // Se o boss pode se mover...
        if (canMove) {
            // Pega a próxima posicao do pilar
            Vector3 target = column.newColumnPos();
            /* 
            // Avisa o Animator da direcao dele (antes de atualiza-la), porem apenas se ele estiver se movendo
            if (movement != Vector3.zero) {
                //animator.SetFloat("LastHorizontal", movement[0]);
                //animator.SetFloat("LastVertical", movement[1]);
            } 
            */
            // O boss corre para o proximo pilar que sera levantado
            movement = GetBestMove(this.transform.position, target);
            // O vetor de movimento tem sua magnitude normalizada para que o boss sempre tenha a mesma velocidade, independente da distancia dele aos pilares ou ao jogador
            movement = movement.normalized;
            // Avisa o Animator da direcao e velocidade atuais do boss
            //animator.SetFloat("Horizontal", movement[0]);
            //animator.SetFloat("Vertical", movement[1]);
            //animator.SetFloat("Speed", movement.sqrMagnitude);
        }
        else {
            // Caso ele nao possa...
            // Fica parado
            movement = Vector3.zero;
            // Avisa ao Animator que ele esta parado
            //animator.SetFloat("Horizontal", 0);
            //animator.SetFloat("Vertical", 0);
            //animator.SetFloat("Speed", 0);
        }
    }

    // Essa funcao e chamada a cada determinado periodo de tempo (usada para coisas que envolvem fisica)
    void FixedUpdate() {
        // Movimenta o boss para perto do player, fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento
        bossRB.velocity = Vector3.SmoothDamp(bossRB.velocity, movement*speed, ref curVelocity, smoothTime);
    }

    Vector3 GetBestMove(Vector3 cur, Vector3 dest) {
        Vector3 ideal = dest - cur;   // Calcula o vetor ideal de movimento
        Debug.Log("Movement vector: " + ideal);
        return ideal;   // O boss pode seguir em linha reta
    }
}
