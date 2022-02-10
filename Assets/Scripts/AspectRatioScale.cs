using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioScale : MonoBehaviour {
  [SerializeField] float initialScale = 0.5f;

  [SerializeField] Vector3 originalScale = Vector3.one;

  private void Awake() {
    transform.localScale = Vector3.zero;
  }

  [EditorButton]
  void SetOriginalScale() {
    originalScale = transform.localScale;
  }

  [EditorButton]
  void ResizeBasedOnScale() {
    transform.localScale = originalScale * initialScale;
  }

  public void ResetScale() {
    initialScale = 1f;
  }

  void Update() {
    transform.localScale = Vector3.Lerp(transform.localScale, originalScale * initialScale, 0.1f);
  }
}