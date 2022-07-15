using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheelContainer : MonoBehaviour {
    private AudioLoudnessDetector _detector;

    private SpinWheel[] _spinWheels;
    // Start is called before the first frame update
    void Start() {
        _detector = GetComponent<AudioLoudnessDetector>();
        _spinWheels = GetComponentsInChildren<SpinWheel>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var wheel in _spinWheels) {
            wheel.direction = _detector.audioBand[0] * 50;
        }
    }
}
