﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossMovement : MonoBehaviour {

    public bool canMove = true;
    public Rigidbody2D bossRB;
    private Transform player;
    public Animator animator;
    [HideInInspector] public Vector3 movement = Vector3.zero;
    public float speed;
    [SerializeField] private float minDistance = 10f;
    private float smoothTime = 0.0001f;
    private Vector3 curVelocity;

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        player = GameObject.FindWithTag("Player").transform;
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
            movement = player.position - this.transform.position;
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
        }
        else {
            // Caso ele nao possa...
            // Fica parado
            movement = Vector3.zero;
            // Avisa ao Animator que ele esta parado
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            animator.SetFloat("Speed", 0);
        }
    }

    // Essa funcao e chamada a cada determinado periodo de tempo (usada para coisas que envolvem fisica)
    void FixedUpdate() {
        // Movimenta o boss para perto do player, fazendo com que o vetor velocidade de seu Rigidbody se aproxime suavemente ao vetor de movimento
        bossRB.velocity = Vector3.SmoothDamp(bossRB.velocity, movement*speed, ref curVelocity, smoothTime);
    }
}