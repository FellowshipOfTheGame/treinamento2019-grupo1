using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    
    public bool canMove = true;
    public Rigidbody2D playerRB;
    public Animator animator;
    public Slider dashBar;
    public GameObject dashEffect;
    private Vector3 movement = Vector3.zero;
    private Vector3 lastMov = Vector3.up;
    [SerializeField] private float speed = 17f;
    [SerializeField] private float dashPower = 10f;
    [SerializeField] private float dashTime = 0.1f;
    [SerializeField] private float dashDelay = 3f;
    private float smoothTime = 0.0001f;
    private float curDashDelay = 0f;
    private float curDashTime = 0f;
    private Vector3 curVelocity;

    // Essa funcao e chamada a cada frame
    void Update() {
        // Avisa o Animator da direcao do jogador (antes de atualiza-la), porem apenas se ele estiver se movendo
        if (movement != Vector3.zero) {
            lastMov = movement;
            animator.SetFloat("LastHorizontal", lastMov[0]);
            animator.SetFloat("LastVertical", lastMov[1]);
        }
        // Se o jogador pode se mover
        if (canMove) {
            // A cada frame, um novo vetor de movimento sera construido, considerando os inputs nas coordenadas vertical e horizontal
            movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
            // O vetor de movimento tem sua magnitude normalizada para que o jogador sempre tenha a mesma velocidade em qualquer direcao
            movement = movement.normalized;
            // Checa se o jogador apertou o botao de dash
            bool wantsToDash = (Input.GetAxisRaw("Dash") == 1);
            // Diminui o tempo de espera para o proximo dash
            if (curDashDelay > 0) curDashDelay -= Time.deltaTime;
            if (curDashTime > 0) curDashTime -= Time.deltaTime;
            // Se o jogador nao esta parado...
            if (movement != Vector3.zero) {
                // Se o jogador quiser dar dash...
                if (wantsToDash) {
                    // Checa se o jogador pode, vendo o seu delay atual de dash
                    if (curDashDelay <= 0) {
                        var newDashEffect = Instantiate(dashEffect, this.transform.position, Quaternion.identity); // Instancia o efeito de dash
                        newDashEffect.transform.parent = gameObject.transform;  // O coloca como filho do jogador (para seguir sua posicao)
                        Destroy(newDashEffect, 0.4f); // Destroi o efeito apos um tempo
                        curDashTime = dashTime;   // Comeca a contagem do tempo que ele vai passar dando dash
                        curDashDelay = dashDelay; // Comeca a contar o delay
                    }
                }
            }
        }
        else movement = Vector3.zero;
        // Avisa o Animator da direcao e velocidade atuais do jogador
        animator.SetFloat("Horizontal", movement[0]);
        animator.SetFloat("Vertical", movement[1]);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        // Atualiza a barra de dash
        dashBar.SetValueWithoutNotify(1 - (curDashDelay/dashDelay));
    }

    public Vector3 GetMovement() {
        return movement;
    }

    public Vector3 GetLastMovement() {
        return lastMov;
    }

    // Essa funcao e chamada a cada determinado periodo de tempo (usada para coisas que envolvem fisica)
    void FixedUpdate() {
        // Checa se o jogador esta no dash ou nao
        if (curDashTime > 0) {
            // Movimenta o jogador fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento (escalado pelo fator de dash)
            playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, (movement)*speed*dashPower, ref curVelocity, smoothTime);
        }
        else {
            // Movimenta o jogador fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento
            playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, movement*speed, ref curVelocity, smoothTime);
        }
    }
}
