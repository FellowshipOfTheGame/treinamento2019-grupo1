using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnManager : MonoBehaviour {

    public Transform[] creationPoints;
    public GameObject columnPrefab;
    [SerializeField] private float tipHeight = 1f;
    private int lastPos, newPos;
    private GameObject newColumn;   // Nao sei se preciso salvar isso ainda (questao da gravidade)
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        // Instancia a primeira pilastra (a de cima do mapa)
        newColumn = Instantiate(columnPrefab, creationPoints[0].position, Quaternion.identity);
        // Guarda a ultima posicao instanciada
        lastPos = 0;
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        // Checa se tem alguma pilastra ativa no jogo
        if (!GameObject.FindWithTag("Column")) {
            // Caso nao tenha, escolhe uma posicao aleatoria entre as possiveis (sem repetir a anterior)
            do {
                newPos = Random.Range(0, creationPoints.Length);
            } while (lastPos == newPos);
            // E instancia um novo pilar
            newColumn = Instantiate(columnPrefab, creationPoints[newPos].position, Quaternion.identity);
            lastPos = newPos;   // Guarda a nova posicao
        }
    }

    public Vector3 newColumnPos() {
        return creationPoints[lastPos].position + Vector3.up*tipHeight;
    }
}
