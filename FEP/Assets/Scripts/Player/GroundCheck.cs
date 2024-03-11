using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool _onGround;
    private float _friction;

    private Vector2 _normal;
    private PhysicsMaterial2D _material;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _onGround = false;
        _friction = 0.0f;
    }

    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            _normal = collision.GetContact(i).normal;
            _onGround |= _normal.y > 0.9f;
        }
    }

    private void RetrieveFriction(Collision2D collision)
    {
        _material = collision.rigidbody.sharedMaterial;

        _friction = 0;

        if ( _material != null )
        {
            _friction = _material.friction;
        }
    }

    public bool GetGround()
    {
        return _onGround;
    }

    public float GetFriction()
    {
        return _friction;
    }
}
