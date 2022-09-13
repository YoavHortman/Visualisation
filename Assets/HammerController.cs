using System;
using UnityEngine;


public enum HammerState {
  IDLE,
  DOWN,
  UP
}
[RequireComponent(typeof(HingeJoint2D))]
public class HammerController : MonoBehaviour {
  private HingeJoint2D _hingeJoint2D;
  private JointMotor2D _motor2D;
  private JointAngleLimits2D _limits2D;
  public HammerState state = HammerState.IDLE;
  private Rigidbody2D rb;
  public float timer = 0;

  void Start() {
    _hingeJoint2D = GetComponent<HingeJoint2D>();
    _motor2D = _hingeJoint2D.motor;
    _limits2D = _hingeJoint2D.limits;
    rb = GetComponent<Rigidbody2D>();
  }

  [EditorButton]
  void FireHammer() {
    if (state == HammerState.IDLE) {
      _motor2D.motorSpeed = -1000;
      _hingeJoint2D.motor = _motor2D;
      state = HammerState.DOWN;
      timer = 0;
    }
  }
  
  [EditorButton]
  void FireHammer2() {
    rb.SetRotation(100);
    // rb.AddForceAtPosition();
  }

  void EndHammer() {
    if (state == HammerState.DOWN && timer > 0.25f) {
      _motor2D.motorSpeed = 1000;
      _hingeJoint2D.motor = _motor2D;
      state = HammerState.UP;
    }
  }

  void UpdateDidFire() {
    if (state == HammerState.UP && Math.Abs(_limits2D.min - _hingeJoint2D.jointAngle) < 0.5f) {
      state = HammerState.IDLE;
      _motor2D.motorSpeed = 0;
      _hingeJoint2D.motor = _motor2D;
    }
  }

  // Update is called once per frame
  void Update() {
    EndHammer();
    UpdateDidFire();
    timer += Time.deltaTime;
  }
}