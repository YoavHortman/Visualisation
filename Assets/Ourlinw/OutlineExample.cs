using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineExample : MonoBehaviour
{
    private Material material;
    private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        var size = Mathf.Sin(Time.time) + 1.0f;
        material.SetFloat(OutlineThickness, size);
    }
}