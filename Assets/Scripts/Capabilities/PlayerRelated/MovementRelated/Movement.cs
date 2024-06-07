using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Input Controller")]
    [SerializeField] private InputController _input = default;
    
    [Header("Input Smoothing")]
    [SerializeField, Range(0, 0.1f)] private float _inputSmoothing = 0.1f;

    [Header("Speed")]
    [SerializeField, Range(0, 20f)] private float _maxGroundSpeed = 4f;
    [SerializeField, Range(0, 20f)] private float _maxAirSpeed = 4f;

    [Header("Acceleration")]
    [SerializeField, Range(0, 50f)] private float _maxGroundAcceleration = 35f;
    [SerializeField, Range(0, 50f)] private float _maxAirAcceleration = 20f;

    [Header("Deceleration")]
    [SerializeField, Range(0, 100f)] private float _groundDamping = 5f;
    [SerializeField, Range(0, 100f)] private float _airDamping = 5f;

    [Header("Acceleration Curves")]
    [SerializeField] private AnimationCurve _groundAccelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve _airAccelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    //Dependencies
    private Ground _groundCheckerObject = default;
    private Rigidbody2D _rb = default;
    private BasicAttackManager _basicAttackScript;
    private Animator _anim;

    //Physics variables
    private Vector2 _velocity = default;
    private Vector2 _desiredVelocity = default;
    private Vector2 _direction = default;
    private float _maxSpeedChange = default;
    private float _acceleration = default;

    //Public state property
    public bool IsIdle { get; private set; }
    
    private bool _isGrounded = false;

    private PlayerActionStates _playerActionStates;



    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundCheckerObject = GetComponent<Ground>();
        _anim = GetComponent<Animator>();
        _playerActionStates = GetComponent<PlayerActionStates>();
        _basicAttackScript = GetComponent<BasicAttackManager>();
    }
    private void Update()
    {
        EstablishHorizontalMovementState();
        
        
        
        ChangeLocalScaleBasedOnDirection();  
        

        
        
        _direction.x = Mathf.SmoothDamp(_direction.x, _input.RetreiveMoveInput(), ref _velocity.x, _inputSmoothing);
                  
        _desiredVelocity = CalculateDesiredVelocity(_direction.x, _groundCheckerObject);        
    }
    private void FixedUpdate()
    {
        _isGrounded = _groundCheckerObject.IsGrounded;
        _velocity = _rb.velocity;

        //Find position in acceleration curve and calculate acceleration factor based on that
        float accelerationFactor = CalculateAccelerationFactor(_isGrounded ? _groundAccelerationCurve : _airAccelerationCurve);

        //Lerp acceleration based on acceleration factor
        _acceleration = CalculateAcceleration(accelerationFactor);
        _maxSpeedChange = _acceleration * Time.fixedDeltaTime;

        MovementWithLinearDamping();

        _rb.velocity = _velocity;
    }

    private void MovementWithLinearDamping()
    {
        //If player is grounded and doesn't give out any input, apply linear damping
        //if the player gives input or isn't grounded, don't apply linear damping
        if (_isGrounded)
        {
            if (_direction.x < .1f || _direction.x > -.1f)
            {
                float linearDamping = _groundDamping * Time.fixedDeltaTime;
                _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, linearDamping);
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
            }
        }
        else
        {
            if (_direction.x < .1f || _direction.x > -.1f)
            {
                float airFriction = _airDamping * Time.fixedDeltaTime;
                _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, airFriction);
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
            }
        }
    }
    private Vector2 CalculateDesiredVelocity(float inputScaleX, Ground groundObject) 
        => new Vector2(inputScaleX, 0f) * Mathf.Max(_isGrounded ? _maxGroundSpeed - groundObject.Friction : _maxAirSpeed, 0f);
    private float CalculateAccelerationFactor(AnimationCurve accelerationCurve)
    {
        var velocityPosition = _velocity.x / (_isGrounded ? _maxGroundSpeed : _maxAirSpeed);
        return accelerationCurve.Evaluate(velocityPosition);
    }
    private float CalculateAcceleration(float accelerationFactor)
        => Mathf.Lerp(0, _isGrounded ? _maxGroundAcceleration : _maxAirAcceleration, accelerationFactor);
    
    private void EstablishHorizontalMovementState()
    {
        if(_input.RetreiveMoveInput() != 0)
        {
            IsIdle = false;
        }
        else
        {
            IsIdle = true;
        }
    }
    private void ChangeLocalScaleBasedOnDirection()
    {
        if(_input.RetreiveMoveInput() <= -.1f && transform.localScale.x != -1)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        else if(_input.RetreiveMoveInput() >= .1f && transform.localScale.x != 1)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
}