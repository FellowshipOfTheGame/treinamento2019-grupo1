using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    private Vector3 movement = Vector3.zero;
    [SerializeField] private float speed = 3f;

    // A cada frame, pego o input do jogador
    void Update() {
        // A cada frame, um novo vetor de movimento sera construido, considerando os inputs nas coordenadas vertical e horizontal
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
    }

    // A cada determinado periodo de tempo, movimento o jogador
    void FixedUpdate() {
        // Mudo a posicao da sua Transform para que ela siga o vetor
        transform.position += movement * speed * Time.fixedDeltaTime;
    }
}
