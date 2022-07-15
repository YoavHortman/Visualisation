using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmpScale : MonoBehaviour {
  [SerializeField] private AudioLoudnessDetector _detector;

  private Vector2 originalPos;

  void Start() {
    originalPos = transform.position;
  }

  // Update is called once per frame
  void Update() {
    var val = Mathf.Clamp(_detector.audioBand[0] * 3, 0.3f, 1);
    var v = Vector2.one * (0.4f * (val)) + Vector2.one * 0.3f;
    // var v = Vector2.Min(Vector2.one * 0.7f,
    // Vector2.Max(Vector2.one * 0.3f, Vector2.one * (_detector.audioBand[0] * 3)));
    var vNext = Vector2.MoveTowards(transform.localScale, v, Time.deltaTime * 5);
    transform.localScale = vNext;
    transform.position = new Vector2(originalPos.x,
      originalPos.y + vNext.y / 2);
  }
}