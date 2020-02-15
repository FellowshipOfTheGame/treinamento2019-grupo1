using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        Button playBtn = this.GetComponent<Button>();
        playBtn.onClick.AddListener( () => { 
            SceneController.instance.StartGame(); 
        });
    }
}
