using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public bool IsGrounded { get; private set; }
    public float Friction { get; private set; }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetreiveFriction(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetreiveFriction(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        IsGrounded = false;
        Friction = 0;
    }
    private void EvaluateCollision(Collision2D collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
            var normal = collision.GetContact(i).normal;
            IsGrounded = normal.y >= 0.9f;
        }
    }
    private void RetreiveFriction(Collision2D collision)
    {
        PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;
        Friction = 0;
        if(material is not null)
        {
            Friction = material.friction;
        }
    }
}
