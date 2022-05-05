using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    private SliderJoint2D _sliderJoint2D;
    [SerializeField] private bool inverted = false;
    [SerializeField] private bool start = false;
    [SerializeField] private bool alwaysOn = false;
    [SerializeField] private float delay = 0;
    void Start()
    {
        _sliderJoint2D = GetComponent<SliderJoint2D>();
        Invoke(nameof(startWithDelay), delay);
        start = false; 
        alwaysOn = true;
    }

    private void startWithDelay()
    {
        start = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        start = true;
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        start = alwaysOn || false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (_sliderJoint2D.limitState)
        {
            case JointLimitState2D.UpperLimit:
            {
                if (!start && !inverted)
                {
                    return;
                }
                var motor = _sliderJoint2D.motor;
                motor.motorSpeed = inverted ? -2 : -10;
                _sliderJoint2D.motor = motor;
                break;
            }
            case JointLimitState2D.LowerLimit:
            {
                if (!start && inverted)
                {
                    return;
                }
                var motor = _sliderJoint2D.motor;
                motor.motorSpeed = inverted ? 10 : 2;
                _sliderJoint2D.motor = motor;
                break;
            }
        }
    }
}
