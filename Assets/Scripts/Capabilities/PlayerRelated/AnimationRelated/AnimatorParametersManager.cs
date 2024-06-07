using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParametersManager : MonoBehaviour
{
    private Animator _anim;
    private Movement _movementScript;
    private Jump _jumpScript;
    private BasicAttackManager _basicAttackScript;
    private PlayerActionStates _actionStateScript;

    private string _currentState;
    

    void Start()
    {
        _anim = GetComponent<Animator>();
        _movementScript = GetComponent<Movement>();
        _jumpScript = GetComponent<Jump>();
        _actionStateScript = GetComponent<PlayerActionStates>();
        _basicAttackScript = GetComponent<BasicAttackManager>();        
    }

    
    void Update()
    {
        Animate(_anim, GetCurrentState());
    }

    private string GetCurrentState()
    {
        if(_actionStateScript.CurrentActionState != ActionState.Inaction)
        {
            
                var additive = _basicAttackScript.CurrentAttackType == AttackType.LightAttack ? "L" : "H";
                var returnString =  additive + $"Attack{_basicAttackScript.CurrentWeaponId}-{_basicAttackScript.CurrentComboIndex + 1}";
                return returnString;
        }
        else if(_jumpScript.CurrentVerticalMovementState != Jump.VerticalMovementState.Stationary)
        {
            if(_jumpScript.CurrentVerticalMovementState == Jump.VerticalMovementState.Falling)
            {
                //return fall anim
                return "Idle";
            }
            else
            {
                //return ascend anim
                return "Idle";
            }
        }
        else
        {
            if(_movementScript.IsIdle)
            {
                //return idle anim
                return "Idle";
            }
            else
            {
                //return run anim
                return "Run";
            }
        }
    }
    private void Animate(Animator anim, string state)
    {
        if(_currentState != state)
        {
            anim.Play(state);
        }
    }
}
