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

    public void StartGame() {
        StartCoroutine(ChangeSceneCor(1));
    }

    public void ChangeScene(int index) {
        StartCoroutine(ChangeSceneCor(index));
    }

    // Corotina que vai para a cena passada por parametro
    public IEnumerator ChangeSceneCor(int index) {
        // Comeca a animacao de transicao (um fade)
        fadeAnimator.SetTrigger("Fade");
        // Espera um tempo para que a transicao ocorra
        yield return new WaitForSeconds(1f);
        // Carrega ela
        SceneManager.LoadScene(index);
        // Decide qual musica de fundo ira tocar
        switch (index) {
            case 1:
                AudioManager.instance.StopAll();
                AudioManager.instance.Play("EntranceMusic");
                break;
            case 2:
                AudioManager.instance.StopAll();
                AudioManager.instance.Play("FirstBossMusic");
                break;
            case 3:
                AudioManager.instance.StopAll();
                AudioManager.instance.Play("DungeonHeartMusic");
                break;
        }
    }
}
