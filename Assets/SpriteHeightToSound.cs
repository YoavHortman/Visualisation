using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHeightToSound : MonoBehaviour {
    private AudioLoudnessDetector _detector;

    private SpriteRenderer[] _sprites; 
    // Start is called before the first frame update
    void Start() {
        _detector = GetComponent<AudioLoudnessDetector>();
        _sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (_sprites.Length == 0) {
            return;
        }

        var mul = 10;
        for (var i = 0; i < _detector.audioBandBuffer.Length; i++) {
            _sprites[i].transform.localScale = new Vector2(1, _detector.audioBandBuffer[i] * mul);
            _sprites[i].transform.position = new Vector2(i * 1.1f - _detector.audioBandBuffer.Length / 2f, _detector.audioBandBuffer[i] * mul / 2 - ResizeListener.screenSizeInWorldCoords.y);
        }
    }
}
