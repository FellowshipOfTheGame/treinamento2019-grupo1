using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    public Rigidbody2D playerRB;
    public Transform attack;
    public Animator animator;
    private Vector3 movement = Vector3.zero;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float dashPower = 10f;
    [SerializeField] private float dashTime = 0.1f;
    [SerializeField] private float dashDelay = 1f;
    private float smoothTime = 0.0001f;
    private float angle;
    private float curDashDelay = 0f;
    private float curDashTime = 0f;
    private Vector3 curVelocity;

    // Essa funcao e chamada a cada frame
    void Update() {
        // Avisa o Animator da direcao do jogador (antes de atualiza-la), porem apenas se o jogador estiver se movendo
        if (movement != Vector3.zero) {
            animator.SetFloat("LastHorizontal", movement[0]);
            animator.SetFloat("LastVertical", movement[1]);
        }
        // A cada frame, um novo vetor de movimento sera construido, considerando os inputs nas coordenadas vertical e horizontal
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        // O vetor de movimento tem sua magnitude normalizada para que o jogador sempre tenha a mesma velocidade em qualquer direcao
        movement = movement.normalized;
        // Avisa o Animator da direcao e velocidade atuais do jogador
        animator.SetFloat("Horizontal", movement[0]);
        animator.SetFloat("Vertical", movement[1]);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        // Checa se o jogador apertou o botao de dash
        bool wantsToDash = (Input.GetAxisRaw("Dash") == 1);
        // Diminui o tempo de espera para o proximo dash
        if (curDashDelay > 0) curDashDelay -= Time.deltaTime;
        if (curDashTime > 0) curDashTime -= Time.deltaTime;
        // Se o jogador nao esta parado...
        if (movement != Vector3.zero) {
            // Calcula o vetor referencia para a proxima rotacao do angulo de ataque
            Vector3 reference = attack.transform.position - this.transform.position;
            // Calcula e guarda o angulo do movimento
            angle = Vector3.SignedAngle(reference, movement, Vector3.forward);
            // Se o jogador quiser dar dash...
            if (wantsToDash) {
                // Checa se o jogador pode, vendo o seu delay atual de dash
                if (curDashDelay <= 0) {
                    curDashTime = dashTime;   // Comeca a contagem do tempo que ele vai passar dando dash
                    curDashDelay = dashDelay; // Comeca a contar o delay
                }
            }
        }
    }

    // Essa funcao e chamada a cada determinado periodo de tempo (usada para coisas que envolvem fisica)
    void FixedUpdate() {
        // Checa se o jogador esta no dash ou nao
        if (curDashTime > 0) {
            // Movimenta o jogador fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento (escalado pelo fator de dash)
            playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, (movement)*speed*dashPower, ref curVelocity, smoothTime);
        }
        else {
            // Movimenta o jogador fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento (unitario)
            playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, movement*speed, ref curVelocity, smoothTime);
        }
        // Faz com que a posicao de ataque siga o angulo do movimento (se o jogador nao esta parado)
        if (movement != Vector3.zero) attack.transform.RotateAround(this.transform.position, Vector3.forward, angle);
    }
}
