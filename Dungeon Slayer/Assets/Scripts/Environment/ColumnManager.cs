using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnManager : MonoBehaviour {
    // OBS: este script pode acabar sendo incorporado no script do boss que fica em cima

    public Transform[] creationPoints;
    public GameObject columnPrefab;
    private int lastPos;
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        // Instancia a primeira pilastra (a de cima do mapa)
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        // Checa se tem alguma pilastra ativa no jogo

        // Se nao tiver, instancia uma em posicao aleatoria

    }
}
