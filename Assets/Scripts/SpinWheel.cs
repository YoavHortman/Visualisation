using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpinWheel : MonoBehaviour {
    private Rigidbody2D rb;
    private float direction = 1;
    private bool isShaking = false;

    private Vector2 shakeDir = Vector2.zero;
    // private Vector2[] v = new []{Vector2.up, Vector2.down};
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    [EditorButton]
    void RandomShit() {
        direction = Random.Range(-1f, 1f);
        Invoke(nameof(RandomShit), 1000);
    }
    [EditorButton]
    void Shake() {
        isShaking = !isShaking;
        shakeDir = Vector2.down;
    }

    // Update is called once per frame
    void Update()
    {
        rb.MoveRotation(rb.rotation - direction * Time.deltaTime * 50);

        if (isShaking) {
            if (shakeDir == Vector2.down) {
                rb.MovePosition(rb.position + shakeDir);
                shakeDir = Vector2.up;
            } else {
                rb.MovePosition(rb.position + shakeDir);
                shakeDir = Vector2.down;
            }
        }
    }
}
