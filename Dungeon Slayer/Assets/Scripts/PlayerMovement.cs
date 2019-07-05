using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    public Rigidbody2D playerRB;
    public Transform attack;
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
        // A cada frame, um novo vetor de movimento sera construido, considerando os inputs nas coordenadas vertical e horizontal
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        // Faz com que o vetor de movimento sempre esteja dentro do circulo unitario, o que implica no jogador ter sempre a mesma velocidade em qualquer direcao
        movement = Vector3.ClampMagnitude(movement, 1f);
        // Checa se o jogador apertou o botao de dash
        bool wantsToDash = (Input.GetAxisRaw("Dash") == 1);
        // Se o jogador nao esta parado...
        if (movement != Vector3.zero) {
            // Calcula o vetor referencia para a proxima rotacao do angulo de ataque
            Vector3 reference = attack.transform.position - this.transform.position;
            // Calcula e guarda o angulo do movimento
            angle = Vector3.SignedAngle(reference, movement, Vector3.forward);
        }
        // Diminui o tempo de espera para o proximo dash
        if (curDashDelay > 0) curDashDelay -= Time.deltaTime;
        if (curDashTime > 0) curDashTime -= Time.deltaTime;
        // Se o jogador quiser dar dash...
        if (wantsToDash) {
            // Checa se o jogador pode, vendo o seu delay atual de dash
            if (curDashDelay <= 0) {
                curDashTime = dashTime;   // Comeca a contagem do tempo que ele vai passar dando dash
                curDashDelay = dashDelay; // Comeca a contar o delay
            }
        }
    }

    // Essa funcao e chamada a cada determinado periodo de tempo (usada para coisas que envolvem fisica)
    void FixedUpdate() {
        // Checa se o jogador esta no dash ou nao
        if (curDashTime > 0) {
            if (movement != Vector3.zero) {
                // Se o jogador nao esta parado, movimenta-o fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento (escalado pelo fator de dash)
                playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, (movement)*speed*dashPower, ref curVelocity, smoothTime);
            }
            else {  
                // Caso ele esteja, o dash vai se dar para frente, na direcao e sentido para o qual ele estiver olhando
                playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, (transform.up)*speed*dashPower, ref curVelocity, smoothTime);
            }
        }
        else {
            // Movimento o jogador fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento
            playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, movement*speed, ref curVelocity, smoothTime);
        }
        // Faz com que a posicao de ataque siga o angulo do movimento (se o jogador nao esta parado)
        if (movement != Vector3.zero) attack.transform.RotateAround(this.transform.position, Vector3.forward, angle);
    }
}
