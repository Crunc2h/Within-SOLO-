using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Roll : MonoBehaviour
{
    [Header("Input Controller")]
    [SerializeField] private InputController _input = default;

    [Header("Roll Parameters")]
    [SerializeField] private float _rollSpeed = 10f;            
    [SerializeField] private float _rollDuration = 0.2f;        
    [SerializeField] private float _rollCooldown = 1f;          

    [Header("Roll Physics")]
    [SerializeField] private AnimationCurve _rollSpeedCurve;    
    [SerializeField] private float _rollGravityScale = 0.5f;     

    //Dependencies
    private Ground _groundCheckerObject;
    private Rigidbody2D _rb;
    private BoxCollider2D _boxColl;

    //Relevant private roll variables
    private Vector2 _rollDirection = Vector2.zero;
    private float _rollTimer = 0f;
    private bool _rollOnCooldown = false;

    //Roll state
    public bool IsRolling { get; private set; } = false;
    
    private bool _isGrounded = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rollTimer = _rollDuration;
        _groundCheckerObject = GetComponent<Ground>();
        _boxColl = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        _isGrounded = _groundCheckerObject.IsGrounded;
        if (_input.RetreiveRollInput() && !_rollOnCooldown && _isGrounded)
        {
            RollAction();
        }
    }
    private void FixedUpdate()
    {
        if (IsRolling)
        {
            _rb.velocity = _rollDirection * CalculateRollSpeed();
            _rollTimer -= Time.fixedDeltaTime;
            if(_rollTimer <= 0f)
            {
                _rollTimer = _rollDuration;
            }
        }
    }

    private void RollAction()
    {
        _rollOnCooldown = true;
        IsRolling = true;
        
        _rollDirection = new Vector2(_input.RetreiveMoveInput(), 0).normalized;
        
        _rollTimer = _rollDuration;
        
        //Add better invulnerability logic
        _boxColl.enabled = false;
        
        
        var originalGravityScale = _rb.gravityScale;
        _rb.gravityScale = _rollGravityScale;
        
        StartCoroutine(RollCooldown());
        StartCoroutine(RollDuration(originalGravityScale));
    }
    private IEnumerator RollDuration(float originalGravityScale)
    {
        yield return new WaitForSeconds(_rollDuration);
        _boxColl.enabled = true;
        _rb.gravityScale = originalGravityScale;
        IsRolling = false;
    }
    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(_rollCooldown);
        _rollOnCooldown = false;
    }
    private float CalculateRollSpeed() => _rollSpeed * (_rollSpeedCurve.Evaluate(1.0f - _rollTimer / _rollDuration));
}