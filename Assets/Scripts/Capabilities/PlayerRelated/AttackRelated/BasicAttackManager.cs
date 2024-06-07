using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicAttackManager : MonoBehaviour
{
    public int CurrentWeaponId = default;
    public int CurrentComboIndex { get; private set; } = 0;
    public bool IsAttacking { get; private set; }
    public bool Recovery { get; private set; } = false;  
    
    [SerializeField] private PlayerController _playerController;
    private List<PlayerAttackUnit> _playerAttacks;

    

    [SerializeField] private float _comboResetTime;
    private float _comboTimer;
    private bool _comboActive = false;

    private PlayerActionStates _playerActionStates;
    private Animator _anim;
    public AttackType CurrentAttackType { get; private set; }

    private int _currentAttackCount = default;
    private int _currentRecoveryResetCount = default;
    private int _currentRecoveryEndCount = default;
    
    void Start()
    {
        _playerAttacks = GameObject.FindGameObjectWithTag("PlayerAttacks").GetComponent<PlayerAttacks>().GetPlayerAttacks();
        
        _playerActionStates = GetComponent<PlayerActionStates>();
        _comboTimer = _comboResetTime;

        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (ManageInput())
        {
            if(_playerActionStates.CurrentActionState == ActionState.Inaction && !IsAttacking)
            {
                StartAttack();
            }
        }

        ManageComboTimer();
    }

    private bool ManageInput()
    {
        if(_playerController.RetreiveLightAttackInput())
        {
            CurrentAttackType = AttackType.LightAttack;
            return true;
        }
        else if (_playerController.RetreiveHeavyAttackInput())
        {
            CurrentAttackType = AttackType.HeavyAttack;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartAttack()
    {
        IsAttacking = true;
        _currentAttackCount++;
        
        var attack = _playerAttacks.Where(attack => attack.GetComboIndex() == CurrentComboIndex 
        && attack.GetWeaponId() == CurrentWeaponId).FirstOrDefault();     
        
        //apply attack
        GetComponent<PlayerAttack>().ApplyAttack(attack);
    }

    public void RecoveryStart()
    {      
        if (CurrentComboIndex == _playerAttacks.Where(attack => attack.GetWeaponId() == CurrentWeaponId).Count() - 1)
        {
            CurrentComboIndex = 0;
        }
        else
        {
            CurrentComboIndex++;
        }

        IsAttacking = false;
        Recovery = true;
        
        StartOrContinueCombo();
    }
    public void RecoveryEnd()
    {
        Recovery = false;
        _currentRecoveryEndCount++;
    }

    //Functions to manage combos
    private void ResetCombo()
    {
        _comboActive = false;
        _comboTimer = _comboResetTime;
        CurrentComboIndex = 0;
        
    }
    public void StartOrContinueCombo()
    {
        if(!_comboActive)
        {
            _comboActive= true;
        }
        _comboTimer = _comboResetTime;
    }
    private void ManageComboTimer()
    {
        if (_comboActive && !IsAttacking)
        {
            _comboTimer -= Time.deltaTime;
            if (_comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }


 

    
}

public enum AttackType
{
    LightAttack,
    HeavyAttack
}
