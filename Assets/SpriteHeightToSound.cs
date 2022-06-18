using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode {
    loudnessDetector,
    audioSpecturm
}

public class SpriteHeightToSound : MonoBehaviour {

    public Mode mode = Mode.loudnessDetector;
    private AudioLoudnessDetector _detector;
    private AudioSpectrum _spectrum;
    
    private SpriteRenderer[] _sprites; 
    // Start is called before the first frame update
    void Start() {
        if (mode == Mode.loudnessDetector) {
            _detector = GetComponent<AudioLoudnessDetector>();    
        } else {
            _spectrum = GetComponent<AudioSpectrum>();
        }
        
        _sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (_sprites.Length == 0) {
            return;
        }

        var mul = mode == Mode.loudnessDetector ? 10 : 1000;
        float targetScale;
        Vector2 targetPos;
        var len = mode == Mode.loudnessDetector ? _detector.audioBandBuffer.Length : _spectrum.peakLevels.Length;
        for (var i = 0; i < len; i++) {
            if (mode == Mode.loudnessDetector) {
                targetScale = _detector.audioBandBuffer[i] * mul;
                targetPos = new Vector2(i * 1.1f - _detector.audioBandBuffer.Length / 2f,
                    _detector.audioBandBuffer[i] * mul / 2 - ResizeListener.screenSizeInWorldCoords.y);
            } else {
                targetScale = _spectrum.peakLevels[i] * mul;
                targetPos = new Vector2((i * 1.1f - _spectrum.peakLevels.Length / 2f) ,
                    _spectrum.peakLevels[i] * mul / 2 - ResizeListener.screenSizeInWorldCoords.y * 2);
            }
            
            _sprites[i].transform.localScale = new Vector2(1, targetScale);
            _sprites[i].transform.position = targetPos;
        }
    }
}
