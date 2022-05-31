using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private bool started = false;
    private Material _material;
    private float fade = 1f;
    private static readonly int F = Shader.PropertyToID("_Fade");

    void Start()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

        
        if (started)
        {
            fade = Mathf.Max(0, fade - Time.deltaTime);
                
        }
        else
        {
            fade = Mathf.Min(1, fade + Time.deltaTime);
        }
        _material.SetFloat(F, fade);
        
    }
}
