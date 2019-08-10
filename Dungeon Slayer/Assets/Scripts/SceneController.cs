using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public static SceneController instance;
    
    public Animator fadeAnimator;

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

    public void NextScene() {
        StartCoroutine(NextSceneCor());
    }

    public void PreviousScene() {
        StartCoroutine(PreviousSceneCor());
    }

    // Corotina que vai para a proxima cena (de acordo com a ordem original)
    public IEnumerator NextSceneCor() {
        // Decide qual a proxima cena
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // Comeca a animacao de transicao (um fade)
        fadeAnimator.SetTrigger("Fade");
        // Espera um tempo para que a transicao ocorra
        yield return new WaitForSeconds(1f);
        // Carrega ela
        SceneManager.LoadScene(nextSceneIndex);
        // Decide qual musica de fundo ira tocar
        switch (nextSceneIndex) {
            case 1: 
                break;
            case 2:
                AudioManager.instance.Play("FirstBossMusic");
                break;
        }
    }

    // Corotina que vai para a cena anterior (de acordo com a ordem original)
    public IEnumerator PreviousSceneCor() {
        // Decide qual a proxima cena
        int prevSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        // Comeca a animacao de transicao (um fade)
        fadeAnimator.SetTrigger("Fade");
        // Espera um tempo para que a transicao ocorra
        yield return new WaitForSeconds(1f);
        // Carrega ela
        SceneManager.LoadScene(prevSceneIndex);
        // Decide qual musica de fundo ira tocar
        // (Tera que ter um controle melhor aqui, visto que nos nao queremos que a musica do boss toque de novo quando chegarmos na cena dele)
    }
}
