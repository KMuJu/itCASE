using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    
    public Vector3 CharacterVelocity { get; set; }
            public bool IsGrounded { get; private set; }
            public bool HasJumpedThisFrame { get; private set; }
            public bool IsDead { get; private set; }
            public bool IsCrouching { get; private set; }
    private void Start()
    {
        
    }

    private void Update()
    {
        HasJumpedThisFrame = false;

        bool wasGrounded = IsGrounded;
        GroundCheck();

        // landing
        if (IsGrounded && !wasGrounded)
        {
        }
        // HandleCharacterMovement();
    }

    private void GroundCheck()
    {
        
    }
}
