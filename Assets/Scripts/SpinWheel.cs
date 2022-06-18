using System.Collections;
using System.Collections.Generic;
using Lasp;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpinWheel : MonoBehaviour {
    private Rigidbody2D rb;
    public float direction = 1;
    private bool isShaking = false;
    // public float spinSpeed = 0;

    private Vector2 shakeDir = Vector2.down;
    // private Vector2[] v = new []{Vector2.up, Vector2.down};
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    [EditorButton]
    void RandomShit() {
        direction = Random.Range(-10f, 10f);
        // Invoke(nameof(RandomShit), 1000);
    }
    [EditorButton]
    void Shake() {
        isShaking = !isShaking;
    }

    private float shakeMod = 0.2f;
    private float shakeSpeedMod = 0.2f;
    // Update is called once per frame
    void Update()
    {
        rb.MoveRotation(rb.rotation - direction);

        if (isShaking) {
            if (shakeDir == Vector2.down) {
                rb.MovePosition(rb.position + shakeDir * shakeMod);
                shakeDir = Vector2.up;
                isShaking = false;
                Invoke(nameof(Shake), shakeSpeedMod);
            } else {
                rb.MovePosition(rb.position + shakeDir * shakeMod);
                shakeDir = Vector2.down;
                isShaking = false;
                Invoke(nameof(Shake), shakeSpeedMod);
                shakeMod = Random.Range(0.1f, 0.3f);
                shakeSpeedMod = Random.Range(0.1f, 0.3f);
            }
        }
    }
}
