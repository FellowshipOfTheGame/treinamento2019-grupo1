using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public static SceneController instance;

    // Essa funcao e chamada quando o objeto e instanciado pela primeira vez
    void Awake() {
        // So pode existir uma instancia dessa classe
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
        // Este objeto (SceneController) ira persistir por todas as cenas
        DontDestroyOnLoad(this.gameObject);
    }
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        NextScene();
    }

    // Vai para a proxima cena (de acordo com a ordem original)
    public void NextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        AudioManager.instance.Play("FirstBossMusic"); 
    }
}
