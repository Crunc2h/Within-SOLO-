using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionStates : MonoBehaviour
{
    public ActionState CurrentActionState { get; private set; }
    private Roll _rollScript;
    private BasicAttackManager _basicAttackManager;
    void Start()
    {
        _rollScript = GetComponent<Roll>();
        _basicAttackManager = GetComponent<BasicAttackManager>();
    }

    void Update()
    {
        SetCurrentActionState();
    }


    private void SetCurrentActionState()
    {
        if(_basicAttackManager.IsAttacking)
        {
            CurrentActionState = ActionState.Attacking;
        }
        else if(_rollScript.IsRolling)
        {
            CurrentActionState = ActionState.Rolling;
        }
        else
        {
            CurrentActionState = ActionState.Inaction;
        }
    }

}


public enum ActionState
{
    Inaction,
    Attacking,
    Rolling
}