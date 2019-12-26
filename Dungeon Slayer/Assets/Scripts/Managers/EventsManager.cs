using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager current;

    public void Awake() {
        current = this;
    }

    public event Action onColumnDestroy;
    public void ColumnDestroy() {
        if (onColumnDestroy != null) onColumnDestroy();
    }

    public event Action onFirstBossHit;
    public void FirstBossHit() {
        if (onFirstBossHit != null) onFirstBossHit();
    }
}
