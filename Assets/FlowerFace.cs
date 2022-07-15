using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlowerFace : MonoBehaviour {
  [SerializeField] private AudioLoudnessDetector _spectrum;
  [SerializeField] float VelocityModifier = 1;
  [SerializeField] private float radius = 10;

  private bool toCenter = true;
  private Vector2 targetPos;
  private Vector2 center;
  private void Start() {
    targetPos = Random.insideUnitCircle * radius + (Vector2)transform.position;
    center = transform.position;
  }

  // Update is called once per frame
  void Update() {
    var currVel = VelocityModifier* _spectrum.audioBand[0];
                  
    transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * currVel);
    
    if (PatternUtils.DidReach(transform, targetPos, 0.001f)) {
      if (!toCenter) {
        targetPos = Random.insideUnitCircle * radius + (Vector2)transform.position;
      } else {
        targetPos = center;
      }

      toCenter = !toCenter;
    }
  }
}