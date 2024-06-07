using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAttackUnit : MonoBehaviour
{
    [SerializeField] private int _usedWeaponId = 0;
    [SerializeField] private int _comboIndex = 0;
    [SerializeField] private float _attackDamage = 0f;
    [SerializeField] private Vector2 _hitBox = new Vector2(1,1);
    
    public int GetWeaponId() => _usedWeaponId;
    public int GetComboIndex() => _comboIndex;
    public float GetAttackDamage() => _attackDamage;
    public Vector2 GetHitBox() => _hitBox;

    private void OnDrawGizmosSelected()
    {
        if (Selection.activeGameObject != transform.gameObject)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _hitBox);
    }
}
