using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEmitter : MonoBehaviour
{
    [SerializeField] private Emittable[] toEmit;
    [SerializeField] private float volume = 10;
    [SerializeField] private float strength = 1;
    [SerializeField] private float time = 1;
    [SerializeField] private float lifeTime = 10;
    private CollisionSensor _sensor;
    
    void Start()
    {
        _sensor = GetComponent<CollisionSensor>();
        Invoke(nameof(emitOne), 1);
    }
    
    Emittable getRandomToEmit()
    {
        return toEmit[Random.Range(0, toEmit.Length)];
    }
    
    
    [EditorButton]
    void emitOne()
    {
        // if (!_sensor.isOverlapping) {
            var emitted = Instantiate<Emittable>(getRandomToEmit(), transform);
            emitted.lifeTime = lifeTime;
            emitted.Emit(strength);
            Invoke(nameof(emitOne), time);
        // } else {
            // Debug.Log("retrying" + gameObject.name);
            // Invoke(nameof(emitOne), 0.1f);
        // }
    }

    void Update()
    {
        
    }
}
