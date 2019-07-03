using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    private Vector3 movement = Vector3.zero;
    [SerializeField] private float speed = 3f;
    public Transform attack;
    private float angle;

    // Essa funcao e chamada a cada frame
    void Update() {
        // A cada frame, um novo vetor de movimento sera construido, considerando os inputs nas coordenadas vertical e horizontal
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
        // Calcula o vetor referencia para a proxima rotacao do angulo de ataque
        Vector3 reference = attack.transform.position - this.transform.position;
        // Se o jogador nao esta parado...
        if (movement != Vector3.zero) {
            // Calcula e guarda o angulo do movimento
            angle = Vector3.SignedAngle(reference, movement, Vector3.forward);
        }
    }

    // Essa funcao e chamada a cada determinado periodo de tempo (usada para coisas que envolvem fisica)
    void FixedUpdate() {
        // Muda a posicao da Transform do jogador para que ela siga o vetor (trocar para AddForce)
        transform.position += movement * speed * Time.fixedDeltaTime;
        // Faz com que a posicao de ataque siga o angulo do movimento (se o jogador nao esta parado)
        if (movement != Vector3.zero) attack.transform.RotateAround(this.transform.position, Vector3.forward, angle);
    }
}
