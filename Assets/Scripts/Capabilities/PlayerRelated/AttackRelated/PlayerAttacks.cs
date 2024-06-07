using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    [SerializeField] private List<PlayerAttackUnit> _playerAttacks;
    private void Awake()
    {
        _playerAttacks = GetComponentsInChildren<PlayerAttackUnit>().ToList();
    }
    public List<PlayerAttackUnit> GetPlayerAttacks() => _playerAttacks;
}
