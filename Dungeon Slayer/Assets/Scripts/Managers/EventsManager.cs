using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager current;

    public void Awake() {
        // So pode existir uma instancia dessa classe
        if (current == null) {
            current = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
        // Este objeto ira persistir por todas as cenas
        DontDestroyOnLoad(this.gameObject);
    }

    public event Action onColumnDestroy;
    public void ColumnDestroy() {
        if (onColumnDestroy != null) onColumnDestroy();
    }

    public event Action onFirstBossHit;
    public void FirstBossHit() {
        if (onFirstBossHit != null) onFirstBossHit();
    }

    public event Action onHeartDestroy;
    public void HeartDestroy() {
        if (onHeartDestroy != null) onHeartDestroy();
    }

    public event Action onPlayerDestroy;
    public void PlayerDestroy() {
        if (onPlayerDestroy != null) onPlayerDestroy();
    }
}
