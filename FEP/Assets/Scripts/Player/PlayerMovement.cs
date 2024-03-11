using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputController _controller = null;
    [SerializeField, Range(0f, 100f)][Tooltip("Maximum speed the player can reach")] private float _maxSpeed = 4;
    [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)] private float _maxDecceleration = 35f;
    [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 20f;
    [SerializeField, Range(0f, 100f)] private float _maxAirDecceleration = 20f;
    [SerializeField, Range(0f, 100f)] private float _maxTurnSpeed = 80f;
    [SerializeField, Range(0f, 100f)] private float _maxAirTurnSpeed = 80f;

    private Vector2 _direction, _desiredVelocity, _velocity;
    private Rigidbody2D _body;
    private GroundCheck _ground;

    private float _maxSpeedChange;
    private float _acceleration, _decceleration, _turnSpeed;
    private bool _isGrounded, _pressingKey;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<GroundCheck>();
    }

    private void Update()
    {
        _direction.x = _controller.RetrieveMoveInput();

        Debug.Log($"Vertical speed: {_body.velocity.y}");

        if (_direction.x != 0)
            _pressingKey = true;
        else
            _pressingKey = false;

        _desiredVelocity = new Vector2(_direction.x, 0) * Mathf.Max(_maxSpeed - _ground.GetFriction(), 0);
    }

    private void FixedUpdate()
    {
        _isGrounded = _ground.GetGround();
        _velocity = _body.velocity;

        _acceleration = _isGrounded ? _maxAcceleration : _maxAirAcceleration;
        _decceleration = _isGrounded ? _maxDecceleration : _maxAirDecceleration;
        _turnSpeed = _isGrounded ? _maxTurnSpeed : _maxAirTurnSpeed;

        if (_pressingKey)
        {
            if (Mathf.Sign(_direction.x) != Mathf.Sign(_velocity.x))
                _maxSpeedChange = _turnSpeed * Time.deltaTime;
            else
                _maxSpeedChange = _acceleration * Time.deltaTime;
        }
        else
            _maxSpeedChange = _decceleration * Time.deltaTime;

        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

        _body.velocity = _velocity;
    }
}
