using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private InputController _controller = null;

    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
    [SerializeField, Range(0, 5)] private int _maxAirJumps = 0;
    [SerializeField, Range(0f, 5f)] private float _fallMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float _upMultiplier = 1.7f;
    [SerializeField, Range(0f, 0.3f)] private float _jumpBufferLimit = 0.15f;
    [SerializeField, Range(0f, 0.3f)] private float _coyoteLimit = 0.2f;

    private Rigidbody2D _body;
    private GroundCheck _ground;
    private Vector2 _velocity;

    private int _jumpPhase, _jumpCount;
    private float _defaultGravityScale, _jumpSpeed, _jumpBufferTimer, _coyoteTimer;

    private bool _jumpInput, _isGrounded, _isJumping;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<GroundCheck>();

        _defaultGravityScale = 1f;
        _jumpBufferTimer = 0;
    }

    private void Update()
    {
        _jumpInput |= _controller.RetrieveJumpInput();
    }

    private void FixedUpdate()
    {
        _isGrounded = _ground.GetGround();
        _velocity = _body.velocity;

        if (_isGrounded && _body.velocity.y == 0)
        {
            _jumpPhase = 0;
            _coyoteTimer = _coyoteLimit;
            _isJumping = false;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        if (_jumpInput)
        {
            _jumpInput = false;
            _jumpBufferTimer = _jumpBufferLimit;
        }
        else if (!_jumpInput && _jumpBufferTimer > 0)
        {
            _jumpBufferTimer -= Time.deltaTime;
        }

        if (_jumpBufferTimer > 0)
        {
            JumpAction();
        }
        

        if (_controller.RetrieveJumpHold() && _body.velocity.y > 0)
        {
            _body.gravityScale = _upMultiplier;
        }
        else if (!_controller.RetrieveJumpHold() || _body.velocity.y < 0)
        {
            _body.gravityScale = _fallMultiplier;
        }
        else if (_body.velocity.y == 0)
        {
            _body.gravityScale = _defaultGravityScale;
        }

        _body.velocity = _velocity;
    }

    private void JumpAction()
    {
        if (_coyoteTimer > 0 || (_jumpPhase < _maxAirJumps && _isJumping))
        {
            if (_isJumping)
            {
                _jumpPhase++;
            }

            _jumpBufferTimer = 0;
            _coyoteTimer = 0;
            _isJumping = true;

            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);

            if (_velocity.y > 0f)
            {
                _jumpSpeed = Mathf.Max(_jumpSpeed, _velocity.y, 0f);
            }
            else if (_velocity.y < 0f)
            {
                _jumpSpeed += Mathf.Abs(_body.velocity.y);
            }

            _jumpCount++;
            Debug.Log($"Jumps Count: {_jumpCount}");

            _velocity.y += _jumpSpeed;
        }
    }
}

