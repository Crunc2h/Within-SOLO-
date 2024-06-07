using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [Header("Input Controller")]
    [SerializeField] private InputController _input = default;

    [Header("Jump Variables")]
    [SerializeField, Range(0, 5f)] private float _jumpCooldown = 1f;
    [SerializeField, Range(0, 20f)] private float _jumpHeight = 10f;

    [Header("Variable Jump Height")]
    [SerializeField] private bool _variableJumpHeight = false;
    [SerializeField] private float _variableJumpHeightFactor = 0.5f;

    [Header("Coyote Time")]
    [SerializeField] private bool _coyoteTime = false;
    [SerializeField] private float _coyoteTimeDuration = 0.2f;
    private bool _coyoteLock = false;
    private bool _wasGrounded = false;

    [Header("Jump Buffering")]
    [SerializeField] private bool _jumpBuffering = false;
    [SerializeField] private float _jumpBufferingDuration = 0.1f;
    private bool _jumpInputBuffer = false;

    [Header("Variable Gravity")]
    [SerializeField, Range(0, 10f)] private float _defaultGravity = 2f;
    [SerializeField, Range(0, 10f)] private float _upwardGravity = 1.75f;
    [SerializeField, Range(0, 10f)] private float _downwardGravity = 2.5f;
    
    //Dependencies
    private Ground _groundCheckerObject = default;
    private Rigidbody2D _rb = default;
    
    //Physics variables
    private Vector2 _velocity = default;

    //Public state property
    public VerticalMovementState CurrentVerticalMovementState { get; private set; } = VerticalMovementState.Stationary;

    //Private jump variables
    private bool _jumpInput = false;
    private bool _jumpInputRelease = false;
    private bool _jumpOnCooldown = false;
    
    private bool _isGrounded = false;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundCheckerObject = GetComponent<Ground>();
    }
    private void Update()
    {      
        _isGrounded = _groundCheckerObject.IsGrounded;
        
        if(_isGrounded && !_coyoteLock)
        {
            StartCoroutine(CoyoteDuration());
        }
        
        _velocity = _rb.velocity;

        _jumpInput = _input.RetreiveJumpInput();

        //Handle jumps based on availability of coyote time and jump buffering
        if(_jumpBuffering && _coyoteTime)
        {
            JumpBuffering();
            JumpHandling(_jumpInputBuffer && _wasGrounded && !_jumpOnCooldown, ref _jumpInputBuffer);
        }
        else if(_jumpBuffering && !_coyoteTime)
        {
            JumpBuffering();
            JumpHandling(_jumpInputBuffer && _isGrounded && !_jumpOnCooldown, ref _jumpInputBuffer);
        }
        else if(!_jumpBuffering && _coyoteTime)
        {
            JumpHandling(_jumpInput && _wasGrounded && !_jumpOnCooldown, ref _jumpInput);
        }
        else
        {
            JumpHandling(_jumpInput && _isGrounded && !_jumpOnCooldown, ref _jumpInput);
        }

        VariableJumpHeight();
        AdjustGravityWithVerticalVelocity();

        EstablishVerticalMovementState();

        _rb.velocity = _velocity;  
    }

    private void JumpHandling(bool jumpValidity, ref bool inputToReset)
    {
        if (jumpValidity)
        {
            inputToReset = false;
            JumpAction();
        }
    }
    private void VariableJumpHeight()
    {
        if (_variableJumpHeight)
        {
            _jumpInputRelease = _input.RetreiveJumpInputRelease();
            if (_jumpInputRelease)
            {
                _jumpInputRelease = false;
                _velocity.y *= _variableJumpHeightFactor;
            }
        }
    }
    private void AdjustGravityWithVerticalVelocity()
    {
        if (_rb.velocity.y > 0.1f)
        {
            _rb.gravityScale = _upwardGravity;
        }
        else if (_rb.velocity.y < -0.1f)
        {
            _rb.gravityScale = _downwardGravity;
        }
        else
        {
            _rb.gravityScale = _defaultGravity;
        }
    }
    private void JumpAction()
    {
        _jumpOnCooldown = true;
        
        //To reset vertical velocity if player jumps with coyote time
        if(_coyoteTime && _velocity.y < -.1f)
        {
            _velocity.y = 0;
        }
            
        var jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
        if (_velocity.y > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
        }
        _velocity.y += jumpSpeed;
        
        StartCoroutine(JumpCooldown());
    }
    
    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(_jumpCooldown);
        _jumpOnCooldown = false;
    }
    private IEnumerator CoyoteDuration()
    {
        _coyoteLock = true;
        _wasGrounded = true;
        yield return new WaitForSeconds(_coyoteTimeDuration);
        _wasGrounded = _isGrounded;
        _coyoteLock = false;
    }

    private void JumpBuffering()
    {
        if (_jumpBuffering)
        {
            if (_jumpInput)
            {
                _jumpInput = false;
                _jumpInputBuffer = true;
                StartCoroutine(JumpBufferDuration());
            }
        }
    }
    private IEnumerator JumpBufferDuration()
    {
        yield return new WaitForSeconds(_jumpBufferingDuration);
        _jumpInputBuffer = false;
    }

    private void EstablishVerticalMovementState()
    {
        if (_rb.velocity.y >= .1f)
        {
            CurrentVerticalMovementState = VerticalMovementState.Ascending;
        }
        else if (_rb.velocity.y <= -.1f)
        {
            CurrentVerticalMovementState = VerticalMovementState.Falling;
        }
        else
        {
            CurrentVerticalMovementState = VerticalMovementState.Stationary;
        }
    }

    public enum VerticalMovementState
    {
        Stationary,
        Ascending,
        Falling
    }

}
