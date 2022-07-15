using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlowerPartController : MonoBehaviour {
  [SerializeField] private SpriteRenderer[] parts;
  private ParticleSystem[] _particleSystems;
  [SerializeField] private AudioLoudnessDetector _detector;

  private Color[] baseColorForPart = new Color[8];
  private float[] currValues;

  void Start() {
    baseColorForPart[0] = new Color(211 / 255f, 30 / 255f, 37 / 255f);
    baseColorForPart[1] = new Color(215 / 255f, 163 / 255f, 46 / 255f);
    baseColorForPart[2] = new Color(209 / 255f, 192 / 255f, 43 / 255f);
    baseColorForPart[3] = new Color(54 / 255f, 158 / 255f, 75 / 255f);
    baseColorForPart[4] = new Color(93 / 255f, 181 / 255f, 183 / 255f);
    baseColorForPart[5] = new Color(49 / 255f, 64 / 255f, 123 / 255f);
    baseColorForPart[6] = new Color(138 / 255f, 63 / 255f, 100 / 255f);
    baseColorForPart[7] = new Color(79 / 255f, 46 / 255f, 57 / 255f);
    
    currValues = new float[8];

    _particleSystems = new ParticleSystem[8];

    for (int i = 0; i < parts.Length; i++) {
      _particleSystems[i] = parts[i].GetComponentInChildren<ParticleSystem>();
      var col = _particleSystems[i].colorOverLifetime;

      Gradient grad = new Gradient();
      grad.SetKeys(
        new GradientColorKey[] { new GradientColorKey(baseColorForPart[i], 0.0f), new GradientColorKey(baseColorForPart[i], 1.0f) },
        new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) });

      col.color = grad;
    }
  }


// Update is called once per frame
  void Update() {
    Color color;

    for (var i = 0; i < 8; i++) {
      var x = _detector.audioBand[i] * 3;
      var l = Mathf.Min(0.7f,
        Mathf.Max(0.1f, x));
      // Mathf.Min(0.8f, 0.7f * x * 3 + 0.1f);

      currValues[i] = Mathf.MoveTowards(currValues[i], l, Time.deltaTime * 2);
      if (x < 0.1f) {
        if (_particleSystems.Length > i) {
          _particleSystems[i].Play();
        } else {
          _particleSystems[i].Stop();
        }
      }


      color = baseColorForPart[i] * currValues[i];
      color.a = _detector.amplitudeBuffer;
      parts[i].color = color;
    }

    transform.Rotate(Vector3.forward, Time.deltaTime * 10);
  }
}