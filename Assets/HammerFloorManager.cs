using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerFloorManager : MonoBehaviour {
    [SerializeField]
    private ParticleSystem ps;

    private void OnCollisionEnter2D(Collision2D collision) {
        ps.Play();
    }
}
