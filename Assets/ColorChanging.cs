using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorChanging : MonoBehaviour {
    private SpriteRenderer sr;
    private Color nextTarget = Color.black;
    private float startOfRange = 0;
    private float endOfRange = 10;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        Invoke(nameof(changeTarget), Random.Range(startOfRange, endOfRange));
        Invoke(nameof(ChangeRange), Random.Range(startOfRange, endOfRange));
    }

    void ChangeRange() {
        startOfRange = Random.Range(0, 20);
        endOfRange = Random.Range(startOfRange, 21);
        Invoke(nameof(ChangeRange), 60);
    }

    private void changeTarget() {
        nextTarget = new Color(Random.Range(0f, 0.4f), Random.Range(0f, 0.4f), Random.Range(0f, 0.4f));
        Invoke(nameof(changeTarget), Random.Range(startOfRange, endOfRange));
    }

    void Update() {
        sr.color = Color.Lerp(sr.color, nextTarget, Time.deltaTime * Random.Range(startOfRange, endOfRange));
    }
}